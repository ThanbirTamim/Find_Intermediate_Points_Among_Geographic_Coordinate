using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string Input_Shape_File = @"D:\input_test.shp"; //Input file which will be loaded
            string Output_Shape_File = @"D:\output_test.shp"; //Output File Name Which will be saved
            int distance_between_each_points = 4; //meter unit

            if (Generate(Input_Shape_File, Output_Shape_File, distance_between_each_points))
            {
                MessageBox.Show("Done");
            }
            else
            {
                MessageBox.Show("Fail");
            }
        }

        private bool Generate(string input_Shape_File, string output_Shape_File, int distance_between_each_points)
        {
            try
            {
                var featureset = FeatureSet.Open(input_Shape_File);//Name of your shape file as Input

                List<Coordinate> coords = new List<Coordinate>();
                List<List<Coordinate>> list_coord = new List<List<Coordinate>>();

                foreach (var feature in featureset.Features)
                {
                    for (int i = 0; i < feature.Coordinates.Count - 1; i++)
                    {
                        var coord1 = feature.Coordinates[i];
                        var coord2 = feature.Coordinates[i + 1];
                        var ip = IntermidatePoints(coord1, coord2, distance_between_each_points);
                        coords.AddRange(ip);
                    }
                    list_coord.Add(coords);
                    coords = new List<Coordinate>();
                }

                FeatureSet featureSetNew = new FeatureSet(featureset.FeatureType);
                featureSetNew.Projection = ProjectionInfo.FromEpsgCode(4326);

                foreach (var listcoord in list_coord)
                {
                    if (featureSetNew.FeatureType == FeatureType.Line)
                    {
                        //LineString lineString = new LineString(listcoord);
                        Feature f = new Feature(new LineString(listcoord));
                        featureSetNew.Features.Add(f);
                    }
                    else if (featureSetNew.FeatureType == FeatureType.Polygon)
                    {
                        Feature f = new Feature(new Polygon(new LinearRing(listcoord)));
                        featureSetNew.Features.Add(f);
                    }
                }
                featureSetNew.SaveAs(output_Shape_File, true);//output shape file

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<Coordinate> IntermidatePoints(Coordinate c1, Coordinate c2, int distance_meter)
        {
            /*
             * Note1: If you move 0.000009969 lat or long from source that means you are moving 1 Meter; 
             * Note2: Diagonal value will be 1.42 for 1 meter square area. Formula (c2 = a2 + b2)
             */
            List<Coordinate> intermediatepoint = new List<Coordinate>();
            float x = (float)(c2.X - c1.X);
            float y = (float)(c2.Y - c1.Y);

            if (x >= 0 && y >= 0)//++
            {
                float xDiff = (float)(c2.X - c1.X);
                float yDiff = (float)(c2.Y - c1.Y);
                //var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;//in degree

                intermediatepoint.Add(new Coordinate(c1.X, c1.Y));

                while (xDiff >= 0 && yDiff >= 0)
                {
                    var angle = Math.Atan2(yDiff, xDiff); //in radian
                    var xx = c1.X + ((distance_meter * 0.000009969 * 1.42) * Math.Cos(angle));
                    var yy = c1.Y + ((distance_meter * 0.000009969 * 1.42) * Math.Sin(angle));
                    Coordinate coordinate = new Coordinate(xx, yy);

                    intermediatepoint.Add(coordinate);

                    c1 = coordinate;
                    xDiff = (float)(c2.X - c1.X);
                    yDiff = (float)(c2.Y - c1.Y);
                }
                intermediatepoint.RemoveAt(intermediatepoint.Count - 1);
                intermediatepoint.Add(new Coordinate(c2.X, c2.Y));
            }
            else if (x <= 0 && y <= 0)//--
            {
                float xDiff = (float)(c2.X - c1.X);
                float yDiff = (float)(c2.Y - c1.Y);
                //var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;//in degree

                intermediatepoint.Add(new Coordinate(c1.X, c1.Y));

                while (xDiff <= 0 && yDiff <= 0)
                {
                    var angle = Math.Atan2(yDiff, xDiff); //in radian
                    var xx = c1.X + ((distance_meter * 0.000009969 * 1.42) * Math.Cos(angle));
                    var yy = c1.Y + ((distance_meter * 0.000009969 * 1.42) * Math.Sin(angle));
                    Coordinate coordinate = new Coordinate(xx, yy);

                    intermediatepoint.Add(coordinate);

                    c1 = coordinate;
                    xDiff = (float)(c2.X - c1.X);
                    yDiff = (float)(c2.Y - c1.Y);
                }
                intermediatepoint.RemoveAt(intermediatepoint.Count - 1);
                intermediatepoint.Add(new Coordinate(c2.X, c2.Y));
            }
            else if (x <= 0 && y >= 0)//-+
            {
                float xDiff = (float)(c2.X - c1.X);
                float yDiff = (float)(c2.Y - c1.Y);
                //var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;//in degree

                intermediatepoint.Add(new Coordinate(c1.X, c1.Y));

                while (xDiff <= 0 && yDiff >= 0)
                {
                    var angle = Math.Atan2(yDiff, xDiff); //in radian
                    var xx = c1.X + ((distance_meter * 0.000009969 * 1.42) * Math.Cos(angle));
                    var yy = c1.Y + ((distance_meter * 0.000009969 * 1.42) * Math.Sin(angle));
                    Coordinate coordinate = new Coordinate(xx, yy);

                    intermediatepoint.Add(coordinate);

                    c1 = coordinate;
                    xDiff = (float)(c2.X - c1.X);
                    yDiff = (float)(c2.Y - c1.Y);
                }
                intermediatepoint.RemoveAt(intermediatepoint.Count - 1);
                intermediatepoint.Add(new Coordinate(c2.X, c2.Y));
            }
            else if (x >= 0 && y <= 0)//+-
            {
                float xDiff = (float)(c2.X - c1.X);
                float yDiff = (float)(c2.Y - c1.Y);
                //var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;//in degree

                intermediatepoint.Add(new Coordinate(c1.X, c1.Y));

                while (xDiff >= 0 && yDiff <= 0)
                {
                    var angle = Math.Atan2(yDiff, xDiff); //in radian
                    var xx = c1.X + ((distance_meter * 0.000009969 * 1.42) * Math.Cos(angle));
                    var yy = c1.Y + ((distance_meter * 0.000009969 * 1.42) * Math.Sin(angle));
                    Coordinate coordinate = new Coordinate(xx, yy);

                    //Console.WriteLine(c1.ToString());

                    intermediatepoint.Add(coordinate);

                    c1 = coordinate;
                    xDiff = (float)(c2.X - c1.X);
                    yDiff = (float)(c2.Y - c1.Y);
                }
                intermediatepoint.RemoveAt(intermediatepoint.Count - 1);
                intermediatepoint.Add(new Coordinate(c2.X, c2.Y));
            }

            return intermediatepoint;
        }

    }
}
