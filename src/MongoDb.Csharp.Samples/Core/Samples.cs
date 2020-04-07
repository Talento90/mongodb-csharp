﻿// ReSharper disable InconsistentNaming
namespace MongoDb.Csharp.Samples.Core
{
    public enum Samples
    {
        #region QuickStart

        QuickStart_AccessDatabases,
        QuickStart_AccessCollections,
        QuickStart_InsertDocuments,
        QuickStart_ReadDocuments,
        QuickStart_UpdateDocuments,
        QuickStart_DeleteDocuments,

        #endregion

        #region CRUD

        Crud_Insert_IdMember,
        Crud_Insert_OrderedInsert,
        Crud_Insert_WriteConcern,

        Crud_Read_Basics,
        Crud_Read_Query_ComparisonOperators,
        Crud_Read_Query_LogicalOperators,
        Crud_Read_Query_ElementOperators,
        Crud_Read_Query_EvaluationOperators,
        Crud_Read_Query_ArrayOperators,

        Crud_Update_BasicOperators,
        Crud_Update_ReplaceDocuments,
        Crud_Update_UpdatingArrays,
        #endregion

        #region Aggregation
        Aggregation_Stages_Match,
        Aggregation_Stages_Group,
        Aggregation_Stages_Projection,
        Aggregation_Stages_Unwind,
        Aggregation_Stages_Bucket,
        Aggregation_Stages_Limit_Skip,
        #endregion

        #region Expressions
        Expressions_Slice,
        Expressions_Filter,
        #endregion

        #region Security
        Security_Users,
        Security_Roles
        #endregion
    }
}
