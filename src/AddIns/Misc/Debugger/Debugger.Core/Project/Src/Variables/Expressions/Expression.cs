// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using Ast = ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	/// <summary>
	/// Represents a piece of code that can be evaluated.
	/// For example "a[15] + 15".
	/// </summary>
	public class Expression: DebuggerObject
	{
		public static Expression Empty = new Expression(null);
		
		Ast.Expression expressionAst;
		
		public string Code {
			get {
				if (expressionAst != null) {
					CSharpOutputVisitor csOutVisitor = new CSharpOutputVisitor();
					expressionAst.AcceptVisitor(csOutVisitor, null);
					return csOutVisitor.Text;
				} else {
					return string.Empty;
				}
			}
		}
		
		public Ast.Expression AbstractSynatxTree {
			get { return expressionAst; }
		}
		
		public Expression(ICSharpCode.NRefactory.Ast.Expression expressionAst)
		{
			this.expressionAst = expressionAst;
		}
		
		public static implicit operator Expression(ICSharpCode.NRefactory.Ast.Expression expressionAst)
		{
			return new Expression(expressionAst);
		}
		
		public static implicit operator ICSharpCode.NRefactory.Ast.Expression(Expression expression)
		{
			if (expression == null) {
				return null;
			} else {
				return expression.AbstractSynatxTree;
			}
		}
		
		public override string ToString()
		{
			return this.Code;
		}
		
		static string GetNameFromIndices(uint[] indices)
		{
			string elementName = "[";
			for (int i = 0; i < indices.Length; i++) {
				elementName += indices[i].ToString() + ",";
			}
			elementName = elementName.TrimEnd(new char[] {','}) + "]";
			return elementName;
		}
		
		Expression GetExpressionFromIndices(uint[] indices)
		{
			List<Ast.Expression> indicesAst = new List<Ast.Expression>();
			foreach(uint indice in indices) {
				indicesAst.Add(new Ast.PrimitiveExpression((int)indice, ((int)indice).ToString()));
			}
			return new Ast.IndexerExpression(
				this.Expression,
				indicesAst
			);
		}
	}
}
