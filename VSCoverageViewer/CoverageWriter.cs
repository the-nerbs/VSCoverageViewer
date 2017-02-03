using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using VSCoverageViewer.Models;
using VSCoverageViewer.Properties;
using VSCoverageViewer.Serialization;
using VSCoverageViewer.Serialization.Export;

namespace VSCoverageViewer
{
    class CoverageWriter
    {
        private readonly XmlDocument _schemaDoc;

        public CoverageWriter()
        {
            _schemaDoc = new XmlDocument();
            _schemaDoc.LoadXml(Resources.Schema);
        }


        public void WriteCoverageXml(IEnumerable<CoverageNodeModel> models, string path)
        {
            // Here be a bit of ugly: I needed to jump through a couple hoops to get
            // this to be in a binary-same format as Visual Studio outputs.
            //
            //  1) Because XmlSerializer cannot write out a schema like the CoverageDSPriv files have
            //     the coverage XML is first written to a temp buffer.
            //TODO: temp buffer should probably be a temp file to support large coverage data sets. 
            //
            //  2) Had to add the Namespaces\NamespacesInternal properties to CoverageDSPriv to
            //     make XmlSerializer _not_ write out the xsi and xsd namespaces.

            var ser = new XmlSerializer(typeof(CoverageDSPriv));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                OmitXmlDeclaration = true,
                Encoding = Encoding.Unicode,
            };


            CoverageDSPriv merged = ConcatenateFiles(models.Select(CreateSerializable));

            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, settings))
            {
                ser.Serialize(writer, merged, CoverageDSPriv.NamespacesInternal);
            }

            var itemDoc = new XmlDocument();
            itemDoc.LoadXml(builder.ToString());
            itemDoc.DocumentElement.PrependChild(
                itemDoc.ImportNode(_schemaDoc.DocumentElement, true)
            );

            using (var writer = XmlWriter.Create(path, settings))
            {
                itemDoc.Save(writer);
            }
        }

        public void WriteHtmlSummary(IEnumerable<CoverageNodeModel> models, string name, string path)
        {
            CoverageDSPriv merged = ConcatenateFiles(models.Select(CreateSerializable));
            var exportModel = new CoverageExport(name, merged);

            var ser = new XmlSerializer(typeof(CoverageExport));

            string tempFilePath = Path.GetTempFileName();
            using (var tempFile = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                ser.Serialize(tempFile, exportModel);
            }

            var transform = new XslCompiledTransform();
            using (var xslReader = XmlReader.Create(new StringReader(Resources.HTMLTransform)))
            {
                transform.Load(xslReader);
            }

            var args = new XsltArgumentList();
            args.AddParam("genDate", "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
            args.AddParam("totalLines", "", exportModel.LinesCovered + exportModel.LinesPartiallyCovered + exportModel.LinesNotCovered);
            args.AddParam("totalBlocks", "", exportModel.BlocksCovered + exportModel.BlocksNotCovered);

            using (var outputFile = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                transform.Transform(tempFilePath, args, outputFile);
            }
        }


        private static CoverageDSPriv ConcatenateFiles(IEnumerable<CoverageDSPriv> files)
        {
            var modules = new List<ModuleCoverageInfo>();
            var sourceFiles = new List<SourceFileInfo>();


            foreach (var origFile in files)
            {
                int sourceIndexOffset = sourceFiles.Count;

                foreach (var mod in origFile.Modules)
                {
                    var modCopy = mod.Clone();
                    modCopy.OffsetSourceFileIDs(sourceIndexOffset);
                    modules.Add(modCopy);
                }

                foreach (var sourceFile in origFile.SourceFileNames)
                {
                    var copy = sourceFile.Clone();
                    copy.FileID += sourceIndexOffset;
                    sourceFiles.Add(copy);
                }
            }

            return new CoverageDSPriv
            {
                Modules = modules.ToArray(),
                SourceFileNames = sourceFiles.ToArray(),
            };
        }


        private static CoverageDSPriv CreateSerializable(CoverageNodeModel model)
        {
            var file = new CoverageDSPriv();

            file.Modules         = model.Children.Select(CreateSerializableModule).ToArray();
            file.SourceFileNames = (SourceFileInfo[])(model.AdditionalData["SourceFileNames"]);

            return file;
        }

        private static ModuleCoverageInfo CreateSerializableModule(CoverageNodeModel model)
        {
            var module = new ModuleCoverageInfo();

            module.ModuleName            = model.Name;
            module.ImageSize             = (int)model.AdditionalData[nameof(module.ImageSize)];
            module.ImageLinkTime         = (int)model.AdditionalData[nameof(module.ImageLinkTime)];
            module.LinesCovered          = model.LinesCovered;
            module.LinesPartiallyCovered = model.LinesPartiallyCovered;
            module.LinesNotCovered       = model.LinesNotCovered;
            module.BlocksCovered         = model.BlocksCovered;
            module.BlocksNotCovered      = model.BlocksNotCovered;

            module.Namespaces = model.Children.Select(CreateSerializableNamespace).ToArray();

            return module;
        }

        private static NamespaceCoverageInfo CreateSerializableNamespace(CoverageNodeModel model)
        {
            var ns = new NamespaceCoverageInfo();

            ns.BlocksCovered         = model.BlocksCovered;
            ns.BlocksNotCovered      = model.BlocksNotCovered;
            ns.LinesCovered          = model.LinesCovered;
            ns.LinesNotCovered       = model.LinesNotCovered;
            ns.LinesPartiallyCovered = model.LinesPartiallyCovered;
            ns.ModuleName            = (string)model.AdditionalData[nameof(ns.ModuleName)];
            ns.NamespaceKeyName      = (string)model.AdditionalData[nameof(ns.NamespaceKeyName)];
            ns.NamespaceName         = model.Name;

            ns.Classes = model.Children.Select(CreateSerializableClass).ToArray();

            return ns;
        }

        private static ClassCoverageInfo CreateSerializableClass(CoverageNodeModel model)
        {
            var cls = new ClassCoverageInfo();

            cls.ClassKeyName = (string)model.AdditionalData[nameof(cls.ClassKeyName)];
            cls.ClassName = model.Name;

            cls.LinesCovered          = model.LinesCovered;
            cls.LinesNotCovered       = model.LinesNotCovered;
            cls.LinesPartiallyCovered = model.LinesPartiallyCovered;
            cls.BlocksCovered         = model.BlocksCovered;
            cls.BlocksNotCovered      = model.BlocksNotCovered;

            cls.NamespaceKeyName = (string)model.AdditionalData[nameof(cls.NamespaceKeyName)];

            cls.Methods = model.Children.Select(CreateSerializableFunction).ToArray();

            return cls;
        }

        private static MethodCoverageInfo CreateSerializableFunction(CoverageNodeModel model)
        {
            var meth = new MethodCoverageInfo();

            meth.MethodKeyName  = (string)model.AdditionalData[nameof(meth.MethodKeyName)];
            meth.MethodName     = model.Name;
            meth.MethodFullName = (string)model.AdditionalData[nameof(meth.MethodFullName)];

            meth.LinesCovered          = model.LinesCovered;
            meth.LinesPartiallyCovered = model.LinesPartiallyCovered;
            meth.LinesNotCovered       = model.LinesNotCovered;
            meth.BlocksCovered         = model.BlocksCovered;
            meth.BlocksNotCovered      = model.BlocksNotCovered;

            meth.Lines = (LineCoverageInfo[])model.AdditionalData[nameof(meth.Lines)];

            return meth;
        }
    }
}
