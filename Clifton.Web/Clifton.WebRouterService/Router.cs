﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Clifton.Core.ExtensionMethods;
using Clifton.Core.ServiceInterfaces;
using Clifton.Core.ModuleManagement;
using Clifton.Core.Semantics;
using Clifton.Core.ServiceManagement;

using Clifton.WebInterfaces;

namespace Clifton.WebRouterService
{
	public class WebRouterModule : IModule
	{
		public void InitializeServices(IServiceManager serviceManager)
		{
			serviceManager.RegisterSingleton<IPublicRouterService, PublicWebRouter>();
			serviceManager.RegisterSingleton<IAuthenticatingRouterService, AuthenticatingWebRouter>();
		}
	}

	public abstract class RouterBase : ServiceBase
	{
		public Dictionary<string, RouteInfo> Routes { get { return routes; } }

		protected Dictionary<string, RouteInfo> routes = new Dictionary<string, RouteInfo>();
	}

	public class PublicWebRouter : RouterBase, IPublicRouterService
	{
		public override void FinishedInitialization()
		{
			base.FinishedInitialization();
			ISemanticProcessor semProc = ServiceManager.Get<ISemanticProcessor>();
			semProc.Register<WebServerMembrane, PublicRouterReceptor>();
		}

		public void RegisterSemanticRoute<T>(string path) where T : SemanticRoute
		{
			routes[path] = new RouteInfo(typeof(T));
		}
	}

	public class AuthenticatingWebRouter : RouterBase, IAuthenticatingRouterService
	{
		public override void FinishedInitialization()
		{
			base.FinishedInitialization();
			ISemanticProcessor semProc = ServiceManager.Get<ISemanticProcessor>();
			semProc.Register<WebServerMembrane, AuthenticatingRouterReceptor>();
		}

		public void RegisterSemanticRoute<T>(string path, RouteType routeType = RouteType.PublicRoute, uint roleMask = 0) where T : SemanticRoute
		{
			routes[path] = new RouteInfo(typeof(T), routeType, roleMask);
		}
	}
}