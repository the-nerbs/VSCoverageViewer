using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.ViewModels
{
    /// <summary>
    /// Base view model type.
    /// </summary>
    /// <typeparam name="TModel">The type of model this view model binds to.</typeparam>
    internal class BaseViewModel<TModel> : ObservableObject
        where TModel : ObservableObject
    {
        private TModel _model;


        /// <summary>
        /// Gets an IMessenger used to send messages to other parts of the application.
        /// </summary>
        protected IMessenger Messenger { get; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public TModel Model
        {
            get { return _model; }
            set
            {
                var prevModel = _model;
                _model = null;

                if (prevModel != null)
                {
                    DetachModel(prevModel);
                }

                if (value != null)
                {
                    AttachModel(value);
                }

                Set(ref _model, value);
            }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="BaseViewModel{TModel}"/> without a model.
        /// </summary>
        protected BaseViewModel()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BaseViewModel{TModel}"/> with a model.
        /// </summary>
        /// <param name="model">The model to initialize with.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
                         Justification = "Virtual call expected and desired to model-attach logic.")]
        protected BaseViewModel(TModel model)
        {
            Messenger = GalaSoft.MvvmLight.Messaging.Messenger.Default;

            Model = model;
        }


        /// <summary>
        /// Performs initialization for a specific model.
        /// </summary>
        /// <param name="newModel">The model to initialize with.</param>
        /// <remarks>
        /// Use this method to initialize data and attach events as needed.
        /// </remarks>
        protected virtual void AttachModel(TModel newModel)
        { }


        /// <summary>
        /// Performs de-initialization for a specific model.
        /// </summary>
        /// <param name="oldModel">The model to detach from.</param>
        /// <remarks>
        /// If any events were subscribed to or resources allocated in
        /// <see cref="AttachModel(TModel)"/>, they should be freed here.
        /// </remarks>
        protected virtual void DetachModel(TModel oldModel)
        { }
    }
}
