using System;
using System.Threading;
using System.Threading.Tasks;
using EditProfiles.Data;
using OMICRON.OCCenter;

namespace EditProfiles.Operations
{
    /// <summary>
    /// Scans Omicron test documents to locate the user specified keywords.
    /// </summary>
    partial class ProcessFiles : IStartProcessInterface
    {

        #region Properties

        private string OmicronProgramName { get; set; }

        private string OmicronProgramId { get; set; }

        private IAutoDoc OmicronDocument { get; set; }

        #endregion

        #region IStartProcessInterface Members

        /// <summary>
        /// Scans specified Omicron Test Document.
        /// </summary>
        public void Scan ( )
        {

            this.ScanDocument ( );
        }

        #endregion

        #region Private Methods

        private void ScanDocument ( )
        {
            try
            {
                // Omicron Test Module index is between 1 and TestModules.Count
                int currentPosition = 1;
                int totalModuleNumber = this.OmicronDocument.TestModules.Count;

                // Update total module number.
                MyCommons.TotalModuleNumber = totalModuleNumber;
                MyCommons.MyViewModel.ModuleProgressBarMax = totalModuleNumber;
                MyCommons.MyViewModel.UpdateCommand.Execute ( null );

#if DEBUG
                Console.WriteLine ( "SCANNING thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
                Console.WriteLine ( "Total TestModule: {0} ", totalModuleNumber );
#endif

                ParallelOptions parallelingOptions = new ParallelOptions ( );
                parallelingOptions.MaxDegreeOfParallelism = 1;
                parallelingOptions.CancellationToken = MyCommons.CancellationToken;

                IAutoTMs testModules = this.OmicronDocument.TestModules;

                // Parallel.For (fromInclusive Int32, toExclusive Int32, parallelOtions, body)
                // totalModelNumber IS EXCLUSIVE SO MUST ADD 1 TO IT.
                // Otherwise the last Test Module will never be processed.
                Parallel.For ( currentPosition, totalModuleNumber + 1, parallelingOptions, ( testModule ) =>
                {
                    // update current module number.
                    MyCommons.CurrentModuleNumber = testModule;
                    MyCommons.MyViewModel.UpdateCommand.Execute ( null );

                    IAutoTM currentTestModule = testModules.get_Item ( testModule );

                    this.OmicronProgramName = currentTestModule.Name;

                    this.OmicronProgramId = currentTestModule.ProgID;

#if DEBUG
                    Console.WriteLine ( "{0}", new String ( '-', 20 ) );
                    Console.WriteLine ( "SCAN PARALLEL thread: {0}", Thread.CurrentThread.GetHashCode ( ) );
                    Console.WriteLine ( "Connecting to {0} type:   {1}", this.OmicronProgramName, this.OmicronProgramId );
#endif

                    switch ( this.OmicronProgramId )
                    {
                        case ProgId.Execute:

                            // ExeCute Module. Process it
                            Task.Factory.StartNew ( ( ) =>
                                {
                                    try
                                    {

                                        // Polling CancellationToken's status.
                                        // If cancellation requested throw error and exit loop.
                                        if ( MyCommons.CancellationToken.IsCancellationRequested == true )
                                        {
                                            MyCommons.CancellationToken.ThrowIfCancellationRequested ( );
                                        }

                                        // Retrieve parameters and save.
                                        Retrieve ( currentTestModule );
                                    }
                                    catch ( AggregateException ae )
                                    {
                                        foreach ( Exception ex in ae.InnerExceptions )
                                        {
                                            // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                                            ErrorHandler.Log ( ex, this.CurrentFileName );
                                        }
                                        return;
                                    }
                                },
                                MyCommons.CancellationToken )
                                .Wait ( );

                            break;

                        default:
                            // Not supported test module. move on to the next module.
                            break;
                    }
                } );
            }
            catch ( AggregateException ae )
            {
                foreach ( Exception ex in ae.InnerExceptions )
                {
                    // Save to the fileOutputFolder and print to Debug window if the project build is in Debug.
                    ErrorHandler.Log ( ex, this.CurrentFileName );
                }
                return;
            }
        }

        #endregion

    }
}
