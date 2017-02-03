using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.ViewModels
{
    class BaseViewModel<TModel> : ObservableObject
        where TModel : ObservableObject
    {
        private TModel _model;


        protected IMessenger Messenger { get; }

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


        public BaseViewModel()
            : this(null)
        { }

        public BaseViewModel(TModel model)
        {
            Messenger = GalaSoft.MvvmLight.Messaging.Messenger.Default;

            Model = model;
        }


        protected virtual void AttachModel(TModel newModel)
        { }

        protected virtual void DetachModel(TModel oldModel)
        { }
    }
}
