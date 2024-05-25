using CustomLogic.Common;

namespace DirectNet.Net.Greenhouse.GUI;

internal static class Program
{
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