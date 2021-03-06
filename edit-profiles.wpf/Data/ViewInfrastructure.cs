﻿
using EditProfiles.MainModel;
namespace EditProfiles.Data
{
    /// <summary>
    /// Infrastructure of the MVVM model.
    /// </summary>
    public class ViewInfrastructure
    {
        /// <summary>
        /// MainWindow view.
        /// </summary>
        public MainWindow View { get; private set; }

        /// <summary>
        /// MainWindow view model.
        /// </summary>
        public ViewModel ViewModel { get; private set; }

        /// <summary>
        /// MainWindow model.
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Infrastructure. 
        /// </summary>
        /// <param name="view">MainWindow view.</param>
        /// <param name="viewModel">MainWindow view model.</param>
        /// <param name="model">MainWindow model.</param>
        public ViewInfrastructure ( MainWindow view, ViewModel viewModel, Model model )
        {
            View = view;
            ViewModel = viewModel;
            Model = model;
        }
    }
}
