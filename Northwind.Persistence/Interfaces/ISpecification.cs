using Northwind.Persistence.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Northwind.Persistence.Interfaces
{
	/// <summary>
	/// A specification of a query over a repository.
	/// </summary>
	public interface ISpecification<T>
	{
		/// <summary>
		/// The query criteria. E.g.
		/// action => action.Title.StartsWith("Workshop") && action.DueDate == someDate
		/// </summary>
		Expression<Func<T, bool>> Criteria { get; }
		/// <summary>
		/// Entity properties of <see cref="T"/> that
		/// should be included in the result. E.g.
		/// x => x.Project
		/// </summary>
		IEnumerable<Expression<Func<T, object>>> Includes { get; }
		/// <summary>
		/// Entity properties of <see cref="T"/> that
		/// should be included in the result represented as strings. E.g.
		/// "Project"
		/// </summary>
		IEnumerable<string> IncludesAsStrings { get; }
		/// <summary>
		/// The tracking option.
		/// </summary>
		TrackingOption Tracking { get; }
	}
}
