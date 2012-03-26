﻿// # Copyright © 2012, Arnold Zokas
// # All rights reserved. 

using System.Dynamic;
using System.Web.Mvc;
using Machine.Specifications;
using Microsoft.CSharp.RuntimeBinder;
using MvcNavigation.Configuration.Advanced;
using MvcNavigation.Specifications.SpecUtils;

namespace MvcNavigation.Specifications.HtmlHelperExtensionSpecs
{
	[Subject(typeof(HtmlHelperExtensions))]
	public class when_named_menu_is_requested
	{
		static MvcHtmlString menu;

		Because of = () =>
		{
			NavigationConfiguration.Initialise(new Node<TestController>(c => c.RootAction()));
			NavigationConfiguration.Initialise("NamedMenu", new Node<TestController>(c => c.Action1()));

			RendererConfiguration.MenuRenderer = (html, model, maxLevels, renderAllLevels) =>
			{
				const string template = "Title:@Model.Title, maxLevels:@ViewBag.MaxLevels, renderAllLevels:false";
				dynamic viewBag = new ExpandoObject();
				viewBag.MaxLevels = maxLevels;
				viewBag.RenderAllLevels = renderAllLevels;

				var executionResult = InMemoryRazorEngine.Execute(template, model, viewBag, typeof(INode).Assembly, typeof(ExpandoObject).Assembly, typeof(Binder).Assembly);
				return new MvcHtmlString(executionResult.RuntimeResult);
			};

			var htmlHelper = new HtmlHelper(new ViewContext(), new ViewPage());
			menu = htmlHelper.Menu(name: "NamedMenu");
		};

		It should_generate_menu =
			() => menu.ToString().ShouldEqual("Title:Action1, maxLevels:1, renderAllLevels:false");
	}
}