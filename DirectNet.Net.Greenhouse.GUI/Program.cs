using CustomLogic.Common;
using ErabliereAPI.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace DirectNet.Net.Greenhouse.GUI;

internal static class Program
{
    private static ErabliereApiOptionsWithSensors? _options;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var (provider, logger) = ServicesSetup.GetServiceProvider();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Form1? form = default;

        try
        {
            form = new Form1(provider);

            logger.Information("ServiceProvider is ready. Now Running the form.");

            Application.Run(form);
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Exception in the main program");
        }
        finally
        {
            logger.Information("Finalizing DirectNet.Net.GUI app.");
            if (form?.IsDisposed == false)
            {
                logger.Information("Form was not disposed.");
                form.Dispose();
            }
        }

        logger.Information("Terminating DirectNet.Net.GUI app.");
    }
}