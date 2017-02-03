using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Provides infrastructure for implementing and utilizing <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <remarks>
    /// This is similar to the MVVM Light type of the same name, but adds support
    /// for only setting if a value has changed, and for dependent properties.
    /// </remarks>
    abstract class ObservableObject : INotifyPropertyChanged
    {
        // mapping of property -> all properties which depend on it.
        private IReadOnlyDictionary<string, List<string>> _depenentProperties;


        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableObject()
        {
            _depenentProperties = GetPropertyDependencies(GetType());
        }


        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            OnPropertyChanged(propertyName);

            if (_depenentProperties != null)
            {
                List<string> dependents;
                if (_depenentProperties.TryGetValue(propertyName, out dependents))
                {
                    foreach (var prop in dependents)
                    {
                        OnPropertyChanged(prop);
                    }
                }
            }
        }

        protected void SetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            SetIfChanged(ref field, value, EqualityComparer<T>.Default, propertyName);
        }

        protected void SetIfChanged<T>(ref T field, T value, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = "")
        {
            if (!comparer.Equals(field, value))
            {
                Set(ref field, value, propertyName);
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private static IReadOnlyDictionary<string, List<string>> GetPropertyDependencies(Type type)
        {
            var dict = new Dictionary<string, List<string>>();

            foreach (var prop in type.GetProperties())
            {
                var attrs = prop.GetCustomAttributes<DependentOnAttribute>();

                foreach (var dependency in attrs)
                {
                    // make sure we don't have any circular dependencies
                    if (!string.IsNullOrWhiteSpace(dependency.ParentProperty) &&
                        dependency.ParentProperty != prop.Name)
                    {
                        List<string> list;

                        if (!dict.TryGetValue(dependency.ParentProperty, out list))
                        {
                            list = new List<string>();
                            dict[dependency.ParentProperty] = list;
                        }

                        list.Add(prop.Name);
                    }
                }
            }

            return dict.Any() ? dict : null;
        }

    }
}
