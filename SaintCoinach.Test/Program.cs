﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintCoinach.Test {
    class Program {
        static void Main(string[] args) {
            const string DataPath = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack\ffxiv";

            var coll = new IO.PackCollection(DataPath);
            var exColl = new Ex.Relational.RelationalExCollection(coll);
            exColl.ActiveLanguage = Ex.Language.English;
            exColl.Definition = Ex.Relational.Definition.RelationDefinition.Deserialize("ex.yaml");
            exColl.Definition.Compile();

            var sheet = exColl.GetSheet("Item");
            
            //var mdl = mdlF.GetModel();
            //var mdlF = (Graphics.ModelFile)coll.GetFile("chara/monster/m0099/obj/body/b0001/model/m0099b0001.mdl");
            //var mdlF = (Graphics.ModelFile)coll.GetFile("chara/equipment/e0003/model/c0101e0003_top.mdl");
            //var mdl = mdlF.GetModel();
            //var sub = mdl.GetSubModel(0);
            //SaveAsWavefront(sub, "test.obj");

            SaveAsCsv(sheet, "test.csv");

            //Console.ReadLine();
        }

        public static void SaveAsCsv(Ex.Relational.IRelationalSheet sheet, string path) {
            using (var s = new StreamWriter(path, false, Encoding.UTF8)) {
                var indexLine = new StringBuilder("key");
                var nameLine = new StringBuilder("");
                var typeLine = new StringBuilder("int32");

                var colIndices = new List<int>();
                foreach (var col in sheet.Header.Columns) {
                    indexLine.AppendFormat(",{0}", col.Index);
                    nameLine.AppendFormat(",{0}", col.Name);
                    typeLine.AppendFormat(",{0}", col.ValueType);

                    colIndices.Add(col.Index);
                }

                s.WriteLine(indexLine);
                s.WriteLine(nameLine);
                s.WriteLine(typeLine);

                foreach (var row in sheet.GetAllRows()) {
                    s.Write(row.Key);
                    foreach (var col in colIndices) {
                        var v = row[col];

                        if (v == null)
                            s.Write(",");
                        else if (IsUnescaped(v))
                            s.Write(",{0}", v);
                        else
                            s.Write(",\"{0}\"", v.ToString().Replace("\"", "\"\""));
                    }
                    s.WriteLine();
                }
            }
        }
        private static bool IsUnescaped(object self) {
            return (self is Boolean
                || self is Byte
                || self is SByte
                || self is Int16
                || self is Int32
                || self is Int64
                || self is UInt16
                || self is UInt32
                || self is UInt64
                || self is Single
                || self is Double);
        }
        public static void SaveAsWavefront(Graphics.SubModel model, string path) {
            var fi = new FileInfo(path);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (var f = new StreamWriter(path, false, Encoding.ASCII)) {
                int indOff = 1;

                foreach (var mesh in model.Meshes) {
                    var vertices = mesh.GetVertices();
                    foreach (var vertex in vertices) {
                        f.WriteLine("v {0} {1} {2}", vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                        f.WriteLine("vt {0} {1}", vertex.TextureCoordinates0.X, vertex.TextureCoordinates0.Y);
                        f.WriteLine("vn {0} {1} {2}", vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    }

                    var indices = mesh.GetIndices();
                    for (int i = 0; i < indices.Length; i += 3) {
                        var i1 = indices[i + 0] + indOff;
                        var i2 = indices[i + 1] + indOff;
                        var i3 = indices[i + 2] + indOff;

                        f.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", i1, i2, i3);
                    }
                    indOff += vertices.Length;
                }
            }
        }
    }
}
