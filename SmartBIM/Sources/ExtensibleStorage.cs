﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Revit.Addin.RevitTooltip
{
    class ExtensibleStorage
    {
        public static void StoreTooltipInfo(ProjectInfo projectInfo, RevitTooltip settings)
        {
            using (Transaction createSchemaAndStoreData = new Transaction(projectInfo.Document, "tCreateAndStore"))
            {
                createSchemaAndStoreData.Start();

                TooltipSchemaWrapper wrapper = new TooltipSchemaWrapper(settings);
                Schema tooltipSchema = wrapper.GetTooltipSchema();
                Entity entity = new Entity(tooltipSchema);
                foreach (SettingsPropertyValue propertyValue in settings.PropertyValues)
                {
                    Field field = tooltipSchema.GetField(propertyValue.Name);
                    entity.Set<string>(field, propertyValue.PropertyValue.ToString());
                }

                projectInfo.SetEntity(entity);
                createSchemaAndStoreData.Commit();
            }
        }

        public static RevitTooltip GetTooltipInfo(ProjectInfo projectInfo)
        {
            RevitTooltip settings = Revit.Addin.RevitTooltip.RevitTooltip.Default;
            if (projectInfo == null)
            {
                return settings;
            }
            Schema tooltipSchema = Schema.Lookup(TooltipSchemaWrapper.SchemaGuid);
            if (tooltipSchema == null)
            {
                return settings;
            }
            Entity tooltipEntity = projectInfo.GetEntity(tooltipSchema);
            if (tooltipEntity.Schema == null)
            {
                return settings;
            }
            settings.DfServer = tooltipEntity.Get<string>("DfServer");
            settings.DfDB = tooltipEntity.Get<string>("DfDB");
            settings.DfPort = tooltipEntity.Get<string>("DfPort");
            settings.DfUser = tooltipEntity.Get<string>("DfUser");
            settings.DfPassword = tooltipEntity.Get<string>("DfPassword");
            settings.DfCharset = tooltipEntity.Get<string>("DfCharset");
            settings.SqliteFilePath = tooltipEntity.Get<string>("SqliteFilePath");
            settings.SqliteFileName = tooltipEntity.Get<string>("SqliteFileName");
            return settings;
        }
    }

    class TooltipSchemaWrapper
    {
        public static readonly Guid SchemaGuid = new Guid("3A8441FB-1A43-4FB8-A6B8-1BD614060310");
        private RevitTooltip m_settings = null;

        public TooltipSchemaWrapper(RevitTooltip settings)
        {
            m_settings = settings;
        }

        public Schema GetTooltipSchema()
        {
            Schema schema = Schema.Lookup(TooltipSchemaWrapper.SchemaGuid);
            if (schema != null) {
                return schema;
            }
            SchemaBuilder schemaBuilder = new SchemaBuilder(SchemaGuid);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName("RevitTipSettings");
            schemaBuilder.SetDocumentation("this is a schema to store tool tip info");
            foreach (SettingsProperty property in m_settings.Properties)
            {
                schemaBuilder.AddSimpleField(property.Name, typeof(string));
            }
            try
            {
                schema = schemaBuilder.Finish();
            }
            catch (Exception e) {
                throw e;
            }
            return schema;
        }
    }
}
