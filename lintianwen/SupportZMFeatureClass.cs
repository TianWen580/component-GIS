using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace lintianwen
{
    internal class SupportZMFeatureClass
	{
        /// modify the value of Z and M of FeatureClass
        public static IGeometry ModifyGeomtryZMValue(IObjectClass featureClass, IGeometry modifiedGeo)
        {
            IFeatureClass trgFtCls = featureClass as IFeatureClass;
            if (trgFtCls == null) return null;
            string shapeFieldName = trgFtCls.ShapeFieldName;
            IFields fields = trgFtCls.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef pGeometryDef = field.GeometryDef;
            IPointCollection pPointCollection = modifiedGeo as IPointCollection;
            if (pGeometryDef.HasZ)
            {
                IZAware pZAware = modifiedGeo as IZAware;
                pZAware.ZAware = true;
                IZ iz1 = modifiedGeo as IZ;
                //set the value of Z to 0
                iz1.SetConstantZ(0);  
            }
            else
            {
                IZAware pZAware = modifiedGeo as IZAware;
                pZAware.ZAware = false;
            }
            if (pGeometryDef.HasM)
            {
                IMAware pMAware = modifiedGeo as IMAware;
                pMAware.MAware = true;
            }
            else
            {
                IMAware pMAware = modifiedGeo as IMAware;
                pMAware.MAware = false;
            }
            return modifiedGeo;
        }

	}
}
