using DataProcessing.Services.AppManagement;
using DataProcessing.Services.Config;


AppManager app = new AppManager(new ConfigurationManager());
app.Start();
//app.Stop();