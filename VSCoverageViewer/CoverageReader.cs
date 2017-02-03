using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using VSCoverageViewer.Models;
using VSCoverageViewer.Serialization;

namespace VSCoverageViewer
{
    class CoverageReader
    {
        public CoverageNodeModel ReadCoverageXml(string path)
        {
            var ser = new XmlSerializer(typeof(CoverageDSPriv));

            CoverageDSPriv file;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                file = (CoverageDSPriv)ser.Deserialize(fs);
            }

            var model = ReadFile(file);

            model.Name = path;

            model.LinesCovered          = model.Children.Sum(ch => ch.LinesCovered);
            model.LinesPartiallyCovered = model.Children.Sum(ch => ch.LinesPartiallyCovered);
            model.LinesNotCovered       = model.Children.Sum(ch => ch.LinesNotCovered);

            model.BlocksCovered         = model.Children.Sum(ch => ch.BlocksCovered);
            model.BlocksNotCovered      = model.Children.Sum(ch => ch.BlocksNotCovered);

            return model;
        }


        private static CoverageNodeModel ReadFile(CoverageDSPriv file)
        {
            var model = new CoverageNodeModel(CoverageNodeType.CoverageFile);

            foreach (var mod in file.Modules)
            {
                model.Children.Add(ReadModule(mod));
            }

            model.AdditionalData[nameof(file.SourceFileNames)] = file.SourceFileNames;

            return model;
        }

        private static CoverageNodeModel ReadModule(ModuleCoverageInfo mod)
        {
            var model = new CoverageNodeModel(CoverageNodeType.Module);

            model.Name = mod.ModuleName;

            model.AdditionalData[nameof(mod.ImageSize)]     = mod.ImageSize;
            model.AdditionalData[nameof(mod.ImageLinkTime)] = mod.ImageLinkTime;

            model.LinesCovered          = mod.LinesCovered;
            model.LinesPartiallyCovered = mod.LinesPartiallyCovered;
            model.LinesNotCovered       = mod.LinesNotCovered;

            model.BlocksCovered         = mod.BlocksCovered;
            model.BlocksNotCovered      = mod.BlocksNotCovered;

            foreach (var ns in mod.Namespaces)
            {
                model.Children.Add(ReadNamespace(ns));
            }

            return model;
        }

        private static CoverageNodeModel ReadNamespace(NamespaceCoverageInfo ns)
        {
            var model = new CoverageNodeModel(CoverageNodeType.Namespace);

            model.Name = ns.NamespaceName;

            model.BlocksCovered         = ns.BlocksCovered;
            model.BlocksNotCovered      = ns.BlocksNotCovered;

            model.LinesCovered          = ns.LinesCovered;
            model.LinesPartiallyCovered = ns.LinesPartiallyCovered;
            model.LinesNotCovered       = ns.LinesNotCovered;

            model.AdditionalData[nameof(ns.ModuleName)]       = ns.ModuleName;
            model.AdditionalData[nameof(ns.NamespaceKeyName)] = ns.NamespaceKeyName;

            foreach (var cls in ns.Classes)
            {
                model.Children.Add(ReadClass(cls));
            }

            return model;
        }

        private static CoverageNodeModel ReadClass(ClassCoverageInfo cls)
        {
            var model = new CoverageNodeModel(CoverageNodeType.Class);

            model.Name = cls.ClassName;

            model.LinesCovered          = cls.LinesCovered;
            model.LinesPartiallyCovered = cls.LinesPartiallyCovered;
            model.LinesNotCovered       = cls.LinesNotCovered;

            model.BlocksCovered         = cls.BlocksCovered;
            model.BlocksNotCovered      = cls.BlocksNotCovered;

            model.AdditionalData[nameof(cls.ClassKeyName)]     = cls.ClassKeyName;
            model.AdditionalData[nameof(cls.NamespaceKeyName)] = cls.NamespaceKeyName;

            foreach (var meth in cls.Methods)
            {
                model.Children.Add(ReadFunction(meth));
            }

            return model;
        }

        private static CoverageNodeModel ReadFunction(MethodCoverageInfo meth)
        {
            var model = new CoverageNodeModel(CoverageNodeType.Function);

            model.Name = meth.MethodName;

            model.LinesCovered          = meth.LinesCovered;
            model.LinesPartiallyCovered = meth.LinesPartiallyCovered;
            model.LinesNotCovered       = meth.LinesNotCovered;

            model.BlocksCovered         = meth.BlocksCovered;
            model.BlocksNotCovered      = meth.BlocksNotCovered;

            model.AdditionalData[nameof(meth.MethodKeyName)] = meth.MethodKeyName;
            model.AdditionalData[nameof(meth.MethodFullName)] = meth.MethodFullName;

            model.AdditionalData[nameof(meth.Lines)] = meth.Lines;

            return model;
        }
    }
}
