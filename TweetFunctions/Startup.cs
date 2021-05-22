using System;
using System.Collections.Generic;
using System.Text;
using TweetFunctions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TA.Repo.Implementation;
using TA.Repo.Interface;
using TA.Service.Implementation;
using TA.Service.Interface;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TweetFunctions
{    
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();
            //builder.Services.AddSingleton<IRepository, Repository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<ITweetRepository, TweetRepository>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ITweetService, TweetService>();
            // or one of the options below
            // builder.Services.AddScoped<IRepository, Repository>();
            // builder.Services.AddTransient<IRepository, Repository>();
            builder.Services.AddLogging();
        }
    }
}
