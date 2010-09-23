// TestDataset.cs created with MonoDevelop
// User: uhrm at 17:31 09/26/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;

using Hdf5;

namespace Hdf5.Tests
{
    [TestFixture]
    public class TestDataset
    {
        private struct Triplet
        {
            public int i;
            public int j;
            public double v;
            public Triplet(int i, int j, double v) { this.i=i; this.j=j; this.v=v; }
        }
        
        [Test]
        public void TestCreateWithFile()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.Create(h5file, "T1", Hdf5.Datatype.Int32LE, new Hdf5.Dataspace(new ulong[] {128}, new ulong[] {128})));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "T1"))
                {
                    using (Hdf5.Datatype h5tp = h5ds.Type)
                    {
                        Assert.AreEqual(Hdf5.DatatypeClass.Integer, h5tp.Class);
                        Assert.AreEqual(4, h5tp.Size);
                    }
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                        Assert.AreEqual(128, h5sp.GetDimensions()[0]);
                    }
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestCreateWithGroup()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                using (Hdf5.Group h5group = Hdf5.Group.Create(h5file, "G1"))
                {
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.Create(h5group, "T1", Hdf5.Datatype.Int32LE, new Hdf5.Dataspace(new ulong[] {128}, new ulong[] {128})));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "G1/T1"))
                {
                    using (Hdf5.Datatype h5tp = h5ds.Type)
                    {
                        Assert.AreEqual(Hdf5.DatatypeClass.Integer, h5tp.Class);
                        Assert.AreEqual(4, h5tp.Size);
                    }
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                        Assert.AreEqual(128, h5sp.GetDimensions()[0]);
                    }
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_Int32_2D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    int[,] data = new int[,] {{0, 1, 2, 3},
                                              {1, 2, 3, 4},
                                              {2, 3, 4, 5},
                                              {3, 4, 5, 6}};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "T1", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "T1"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(2, h5sp.NumDimensions);
                    }
                    int[,] T1 = (int[,])h5ds.ReadValueArray<int>();
                    Assert.AreEqual(4, T1.GetLength(0));
                    Assert.AreEqual(4, T1.GetLength(1));
                    Assert.AreEqual(0, T1[0,0]);
                    Assert.AreEqual(1, T1[0,1]);
                    Assert.AreEqual(2, T1[0,2]);
                    Assert.AreEqual(3, T1[0,3]);
                    Assert.AreEqual(1, T1[1,0]);
                    Assert.AreEqual(2, T1[1,1]);
                    Assert.AreEqual(3, T1[1,2]);
                    Assert.AreEqual(4, T1[1,3]);
                    Assert.AreEqual(2, T1[2,0]);
                    Assert.AreEqual(3, T1[2,1]);
                    Assert.AreEqual(4, T1[2,2]);
                    Assert.AreEqual(5, T1[2,3]);
                    Assert.AreEqual(3, T1[3,0]);
                    Assert.AreEqual(4, T1[3,1]);
                    Assert.AreEqual(5, T1[3,2]);
                    Assert.AreEqual(6, T1[3,3]);
                    
                    using (Hdf5.Dataspace h5fs = h5ds.Space)
                    {
                        h5fs.SelectHyperslab(SelectOperation.Set, new ulong[] {0,1}, null, new ulong[] {4,1}, null);
                        Assert.IsTrue(h5fs.IsSelectionValid);
                        int[,] V1 = new int[4,1];
                        using (Hdf5.Dataspace h5ms = new Hdf5.Dataspace(new ulong[] {4,1}))
                            h5ds.ReadValueArray<int>(h5ms, h5fs, V1);
                        Assert.AreEqual(1, V1[0,0]);
                        Assert.AreEqual(2, V1[1,0]);
                        Assert.AreEqual(3, V1[2,0]);
                        Assert.AreEqual(4, V1[3,0]);
                    }
                    
                    using (Hdf5.Dataspace h5fs = h5ds.Space)
                    {
                        h5fs.SelectHyperslab(SelectOperation.Set, new ulong[] {2,1}, null, new ulong[] {1,3}, null);
                        Assert.IsTrue(h5fs.IsSelectionValid);
                        int[,] H2 = new int[1,3];
                        using (Hdf5.Dataspace h5ms = new Hdf5.Dataspace(new ulong[] {1,3}))
                            h5ds.ReadValueArray<int>(h5ms, h5fs, H2);
                        Assert.AreEqual(3, H2[0,0]);
                        Assert.AreEqual(4, H2[0,1]);
                        Assert.AreEqual(5, H2[0,2]);
                    }
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_Double_2D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    double[,] data = new double[,] {{ 0.0,  1.0,  2.0,  3.0},
                                                    { 4.0,  5.0,  6.0,  7.0},
                                                    { 8.0,  9.0, 10.0, 11.0},
                                                    {12.0, 13.0, 14.0, 15.0}};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "T2", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "T2"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(2, h5sp.NumDimensions);
                    }
                    double[,] T2 = (double[,])h5ds.ReadValueArray<double>();
                    Assert.AreEqual(4, T2.GetLength(0));
                    Assert.AreEqual(4, T2.GetLength(1));
                    Assert.AreEqual( 0.0, T2[0,0]);
                    Assert.AreEqual( 1.0, T2[0,1]);
                    Assert.AreEqual( 2.0, T2[0,2]);
                    Assert.AreEqual( 3.0, T2[0,3]);
                    Assert.AreEqual( 4.0, T2[1,0]);
                    Assert.AreEqual( 5.0, T2[1,1]);
                    Assert.AreEqual( 6.0, T2[1,2]);
                    Assert.AreEqual( 7.0, T2[1,3]);
                    Assert.AreEqual( 8.0, T2[2,0]);
                    Assert.AreEqual( 9.0, T2[2,1]);
                    Assert.AreEqual(10.0, T2[2,2]);
                    Assert.AreEqual(11.0, T2[2,3]);
                    Assert.AreEqual(12.0, T2[3,0]);
                    Assert.AreEqual(13.0, T2[3,1]);
                    Assert.AreEqual(14.0, T2[3,2]);
                    Assert.AreEqual(15.0, T2[3,3]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_Double_Var()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    double[][] data = new double[][] {new double[] { 0.0},
                                                      new double[] { 1.1,  2.2},
                                                      new double[] {},
                                                      new double[] { 3.3,  4.4,  5.5},
                                                      new double[] { 6.6,  7.7,  8.8,  9.9}};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "T3", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "T3"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                    }
                    double[][] T3 = h5ds.ReadVariableLength<double>();
                    Assert.AreEqual(5, T3.Length);
                    Assert.AreEqual(1, T3[0].Length);
                    Assert.AreEqual(0.0, T3[0][0]);
                    Assert.AreEqual(2, T3[1].Length);
                    Assert.AreEqual(1.1, T3[1][0]);
                    Assert.AreEqual(2.2, T3[1][1]);
                    Assert.AreEqual(0, T3[2].Length);
                    Assert.AreEqual(3, T3[3].Length);
                    Assert.AreEqual(3.3, T3[3][0]);
                    Assert.AreEqual(4.4, T3[3][1]);
                    Assert.AreEqual(5.5, T3[3][2]);
                    Assert.AreEqual(4, T3[4].Length);
                    Assert.AreEqual(6.6, T3[4][0]);
                    Assert.AreEqual(7.7, T3[4][1]);
                    Assert.AreEqual(8.8, T3[4][2]);
                    Assert.AreEqual(9.9, T3[4][3]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_Struct_1D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    Triplet[] data = new Triplet[] {new Triplet(0, 0, 5.0),
                                                    new Triplet(0, 1, 3.0),
                                                    new Triplet(1, 0, 8.0),
                                                    new Triplet(1, 2, 1.0),
                                                    new Triplet(2, 0, 6.0),
                                                    new Triplet(2, 3, 9.0),
                                                    new Triplet(3, 0, 2.0),
                                                    new Triplet(3, 4, 4.0)};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "T4", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "T4"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                    }
                    Triplet[] T4 = (Triplet[])h5ds.ReadValueArray<Triplet>();
                    Assert.AreEqual(8, T4.Length);
                    Assert.AreEqual(  0, T4[0].i);
                    Assert.AreEqual(  0, T4[0].j);
                    Assert.AreEqual(5.0, T4[0].v);
                    Assert.AreEqual(  0, T4[1].i);
                    Assert.AreEqual(  1, T4[1].j);
                    Assert.AreEqual(3.0, T4[1].v);
                    Assert.AreEqual(  1, T4[2].i);
                    Assert.AreEqual(  0, T4[2].j);
                    Assert.AreEqual(8.0, T4[2].v);
                    Assert.AreEqual(  1, T4[3].i);
                    Assert.AreEqual(  2, T4[3].j);
                    Assert.AreEqual(1.0, T4[3].v);
                    Assert.AreEqual(  2, T4[4].i);
                    Assert.AreEqual(  0, T4[4].j);
                    Assert.AreEqual(6.0, T4[4].v);
                    Assert.AreEqual(  2, T4[5].i);
                    Assert.AreEqual(  3, T4[5].j);
                    Assert.AreEqual(9.0, T4[5].v);
                    Assert.AreEqual(  3, T4[6].i);
                    Assert.AreEqual(  0, T4[6].j);
                    Assert.AreEqual(2.0, T4[6].v);
                    Assert.AreEqual(  3, T4[7].i);
                    Assert.AreEqual(  4, T4[7].j);
                    Assert.AreEqual(4.0, T4[7].v);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_String_Var()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    string[] data = new string[] {"String 1",
                                                  "Some other string",
                                                  "",
                                                  "Third string which is quite a bit longer",
                                                  "Last!"};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "S1", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "S1"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                    }
                    string[] S1 = (string[])h5ds.ReadStringArray();
                    Assert.AreEqual(5, S1.Length);
                    Assert.AreEqual("String 1", S1[0]);
                    Assert.AreEqual("Some other string", S1[1]);
                    Assert.AreEqual("", S1[2]);
                    Assert.AreEqual("Third string which is quite a bit longer", S1[3]);
                    Assert.AreEqual("Last!", S1[4]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_String_Var2D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    string[][] data = new string[][] {new string[] {"S(1,1)", "S(1,2)"},
                                                      new string[] {"S(2,1)"},
                                                      new string[] {},
                                                      null,
                                                      new string[] {"S(5,1)", null, "", "S(5,4)"},
                                                      new string[] {"S(6,1)", "S(6,2)", "S(6,3)"}};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "Sx", data));
                }
                
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "Sx"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                    }
                    string[][] Sx = h5ds.ReadStringVlenArray();
                    Assert.AreEqual(6, Sx.Length);
                    
                    Assert.AreEqual(2, Sx[0].Length);
                    Assert.AreEqual("S(1,1)", Sx[0][0]);
                    Assert.AreEqual("S(1,2)", Sx[0][1]);
                    
                    Assert.AreEqual(1, Sx[1].Length);
                    Assert.AreEqual("S(2,1)", Sx[1][0]);
                    
                    Assert.IsNull(Sx[2]);
                    Assert.IsNull(Sx[3]);
                    
                    Assert.AreEqual(4, Sx[4].Length);
                    Assert.AreEqual("S(5,1)", Sx[4][0]);
                    Assert.AreEqual(null, Sx[4][1]);
                    Assert.AreEqual("", Sx[4][2]);
                    Assert.AreEqual("S(5,4)", Sx[4][3]);
                    
                    Assert.AreEqual(3, Sx[5].Length);
                    Assert.AreEqual("S(6,1)", Sx[5][0]);
                    Assert.AreEqual("S(6,2)", Sx[5][1]);
                    Assert.AreEqual("S(6,3)", Sx[5][2]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_String_2D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    string[,] data = new string[,] {{"S(1,1)", "S(1,2)"},
                                                    {"S(2,1)", "S(2,2)"}};
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "S2", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "S2"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(2, h5sp.NumDimensions);
                    }
                    string[,] S2 = (string[,])h5ds.ReadStringArray();
                    Assert.AreEqual(2, S2.GetLength(0));
                    Assert.AreEqual(2, S2.GetLength(1));
                    Assert.AreEqual("S(1,1)", S2[0,0]);
                    Assert.AreEqual("S(1,2)", S2[0,1]);
                    Assert.AreEqual("S(2,1)", S2[1,0]);
                    Assert.AreEqual("S(2,2)", S2[1,1]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
        
        [Test]
        public void TestDataset_BitArray_1D()
        {
            string tmpfile = System.IO.Path.GetTempFileName();
            try
            {
                using (Hdf5.File h5file = Hdf5.File.Create(tmpfile, Hdf5.FileAccessFlags.Truncate))
                {
                    int[] mem = new int[] {0x235897, 0x01234A};
                    BitArray data = new BitArray(mem);
                    using (Hdf5.Dataset h5ds = Hdf5.Dataset.CreateWithData(h5file, "BA1", data));
                }
            
                using (Hdf5.File h5file = Hdf5.File.Open(tmpfile, Hdf5.FileAccessFlags.ReadOnly))
                using (Hdf5.Dataset h5ds = Hdf5.Dataset.Open(h5file, "BA1"))
                {
                    using (Hdf5.Dataspace h5sp = h5ds.Space)
                    {
                        Assert.AreEqual(1, h5sp.NumDimensions);
                    }
                    BitArray data = h5ds.ReadBitArray();
                    Assert.AreEqual(64, data.Length);

                    Assert.AreEqual(true,  data[ 0]); Assert.AreEqual(true,  data[ 1]);
                    Assert.AreEqual(true,  data[ 2]); Assert.AreEqual(false, data[ 3]);
                    Assert.AreEqual(true,  data[ 4]); Assert.AreEqual(false, data[ 5]);
                    Assert.AreEqual(false, data[ 6]); Assert.AreEqual(true,  data[ 7]);
                    
                    Assert.AreEqual(false, data[ 8]); Assert.AreEqual(false, data[ 9]);
                    Assert.AreEqual(false, data[10]); Assert.AreEqual(true,  data[11]);
                    Assert.AreEqual(true,  data[12]); Assert.AreEqual(false, data[13]);
                    Assert.AreEqual(true,  data[14]); Assert.AreEqual(false, data[15]);
                    
                    Assert.AreEqual(true,  data[16]); Assert.AreEqual(true,  data[17]);
                    Assert.AreEqual(false, data[18]); Assert.AreEqual(false, data[19]);
                    Assert.AreEqual(false, data[20]); Assert.AreEqual(true,  data[21]);
                    Assert.AreEqual(false, data[22]); Assert.AreEqual(false, data[23]);
                    
                    Assert.AreEqual(false, data[24]); Assert.AreEqual(false, data[25]);
                    Assert.AreEqual(false, data[26]); Assert.AreEqual(false, data[27]);
                    Assert.AreEqual(false, data[28]); Assert.AreEqual(false, data[29]);
                    Assert.AreEqual(false, data[30]); Assert.AreEqual(false, data[31]);

                    Assert.AreEqual(false, data[32]); Assert.AreEqual(true,  data[33]);
                    Assert.AreEqual(false, data[34]); Assert.AreEqual(true,  data[35]);
                    Assert.AreEqual(false, data[36]); Assert.AreEqual(false, data[37]);
                    Assert.AreEqual(true,  data[38]); Assert.AreEqual(false, data[39]);
                    
                    Assert.AreEqual(true,  data[40]); Assert.AreEqual(true,  data[41]);
                    Assert.AreEqual(false, data[42]); Assert.AreEqual(false, data[43]);
                    Assert.AreEqual(false, data[44]); Assert.AreEqual(true,  data[45]);
                    Assert.AreEqual(false, data[46]); Assert.AreEqual(false, data[47]);
                    
                    Assert.AreEqual(true,  data[48]); Assert.AreEqual(false, data[49]);
                    Assert.AreEqual(false, data[50]); Assert.AreEqual(false, data[51]);
                    Assert.AreEqual(false, data[52]); Assert.AreEqual(false, data[53]);
                    Assert.AreEqual(false, data[54]); Assert.AreEqual(false, data[55]);
                    
                    Assert.AreEqual(false, data[56]); Assert.AreEqual(false, data[57]);
                    Assert.AreEqual(false, data[58]); Assert.AreEqual(false, data[59]);
                    Assert.AreEqual(false, data[60]); Assert.AreEqual(false, data[61]);
                    Assert.AreEqual(false, data[62]); Assert.AreEqual(false, data[63]);
                }
            } finally {
                System.IO.File.Delete(tmpfile);
            }
        }
    }
}
