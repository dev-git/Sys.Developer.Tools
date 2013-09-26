using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Developer.Tools.Bdl.Business;
namespace Sys.Developer.Tools.Fcd.DataTransferObjectMapper
{
    public static class Mapper
    {
        /// <summary>
        /// Transforms a list of SqlServerInstance BO's to SqlServerInstance DTO's.
        /// </summary>
        /// <param name="sqlServerInstances"></param>
        /// <returns></returns>
        public static IList<SqlServerInstanceDto> ToDataTransferObjects(IEnumerable<SqlServerInstance> sqlServerInstances)
        {
            if (sqlServerInstances == null)
            {
                return null;
            }
            return sqlServerInstances.Select(c => ToDataTransferObject(c)).ToList();
        }

        /// <summary>
        /// Tranforms a SqlServerInstance BO to a SqlServerInstance DTO
        /// </summary>
        /// <param name="sqlServerInstance"></param>
        /// <returns></returns>
        public static SqlServerInstanceDto ToDataTransferObject(SqlServerInstance sqlServerInstance)
        {
            if (sqlServerInstance == null)
            {
                return null;
            }

            return new SqlServerInstanceDto
            {
                Name = sqlServerInstance.Name,
                Instance = sqlServerInstance.Instance,
                Machine = sqlServerInstance.Machine,
                IsClustered = sqlServerInstance.IsClustered,
                IsLocal = sqlServerInstance.IsLocal,
                Version = sqlServerInstance.Version
            };
        }

        public static DatabaseDto ToDataTransferObject(DataBase db)
        {
            if (db == null)
            {
                return null;
            }

            return new DatabaseDto
            {
                Name = db.Name,
                IsSystem = db.IsSystem
                //SqlServerInstance = Mapper.ToDataTransferObject(db.SqlServerInstance)
            };
        }

               

        public static TableDto ToDataTransferObject(Table table)
        {
            if (table == null)
            {
                return null;
            }

            return new TableDto
            {
                TableName = table.Name
                /*FieldName = table.Fields.Name,
                FieldType = table.Fields.Type,
                FieldPrecision = table.Fields.Precision,
                FieldScale = table.Fields.Scale,
                FieldIsNullable = table.Fields.IsNullable*/
            };
        }

    }

}
