﻿using System.Linq.Expressions;
using ClosedXML.Report.Excel;
using DynamicExpression = ClosedXML.Report.Utils.DynamicExpression;

//using DynamicExpresso;

namespace ClosedXML.Report.Options
{
    public class SummaryFuncTag : OptionTag
    {
        public override byte Priority
        {
            get { return 50; }
        }

        internal DataSource DataSource { get; set; }

        public override void Execute(ProcessingContext context)
        {
            if (context.Range.RowCount() <= 1)
                return;

            var summ = GetFunc();
            /*var lastAddress = Range.RangeAddress.LastAddress;
            var summRow = Range.Worksheet.Range(lastAddress.RowNumber + 1, Range.RangeAddress.FirstAddress.ColumnNumber, 
                lastAddress.RowNumber + 1, lastAddress.ColumnNumber).Unsubscribed();*/
            var summRow = context.Range.LastRow().Unsubscribed();
            if (summ.FuncNum == 0)
            {
                summRow.Cell(summ.Column).Value = summ.Calculate((IDataSource)context.Value);
            }
            else if (summ.FuncNum > 0)
            {
                var funcRngAddr = context.Range.Offset(0, summ.Column-1, context.Range.RowCount()-1, 1).Unsubscribed().Column(1).Unsubscribed().RangeAddress;
                summRow.Cell(summ.Column).FormulaA1 = string.Format("Subtotal({0},{1})", summ.FuncNum, funcRngAddr.ToStringRelative());
            }
        }

        public SubtotalSummaryFunc GetFunc()
        {
            var func = new SubtotalSummaryFunc(Name, Column);
            if (HasParameter("Over"))
                func.GetExpression = type =>
                {
                    var par = Expression.Parameter(type, "item");
                    return DynamicExpression.ParseLambda(new[] {par}, null, GetParameter("Over"));
                };
            func.DataSource = DataSource;
            return func;
        }
    }
}