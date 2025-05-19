var builder = DistributedApplication.CreateBuilder(args);

var dataServices = builder.AddProject<Projects.ICEBG_Web_DataServices>("DataServices");

builder.AddProject<Projects.ICEBG_Web_UserInterface>("UserInterface")
       .WithExternalHttpEndpoints()
       .WithReference(dataServices)
       .WaitFor(dataServices);

builder.Build().Run();
