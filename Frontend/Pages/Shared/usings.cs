// System
global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Net.Http;
global using System.Net.Http.Headers;
global using System.Web;
global using System.Configuration;

// Dependencies
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

global using Development_Praxisworkshop.Helper;

// AppInsight
global using Microsoft.ApplicationInsights;

// App
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Configuration.AzureAppConfiguration;
global using Microsoft.Azure.AppConfiguration.AspNetCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.HttpsPolicy;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.Extensions.Logging;
global using Microsoft.FeatureManagement;
// Web
global using Microsoft.Identity.Web;
global using Microsoft.Identity.Web.UI;

// Auth
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.OpenIdConnect;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using System.Security.Claims;

// Graph
global using Microsoft.Graph;

// Storage
global using Azure.Storage;
global using Azure.Storage.Blobs;
global using Azure.Storage.Blobs.Models;

// Storage - Tables | Deprecated but works
global using Azure.Data.Tables;

// Storage - Queues
global using Azure.Storage.Queues;

// Diagnostics
global using System.Diagnostics;

// Managed Identity / KeyVault
global using Azure;
global using Azure.Identity;
global using Azure.Security.KeyVault.Secrets;
global using Microsoft.Identity.Client;

// Events

global using Microsoft.AspNetCore.SignalR;
global using Development_Praxisworkshop.Hubs;
global using Development_Praxisworkshop.Models;


// Cache
global using Microsoft.Extensions.Caching.StackExchangeRedis;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Identity.Web.TokenCacheProviders.Distributed;

// EventHub
global using Microsoft.Azure.EventHubs;
global using Microsoft.Azure.EventHubs.Processor;