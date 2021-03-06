﻿using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Functions.Cli.Arm;
using Azure.Functions.Cli.Helpers;
using Azure.Functions.Cli.Interfaces;
using Fclp;

namespace Azure.Functions.Cli.Actions.AzureActions
{
    [Action(Name = "login", Context = Context.Azure, HelpText = "Log in to an Azure account")]
    class LoginAction : BaseAzureAccountAction
    {
        private string _username = string.Empty;
        private string _password = string.Empty;

        public LoginAction(IArmManager armManager, ISettings settings)
            : base(armManager, settings, requiresLogin: false)
        { }

        public override ICommandLineParserResult ParseArgs(string[] args)
        {
            Parser.Setup<string>('u', "username")
                .WithDescription("Username to use for non-interactive login. Note that accounts with 2 factor-auth require interactive login. Default: interactive.")
                .Callback(username => _username = username);

            Parser.Setup<string>('w', "password")
                .WithDescription("Password to use for non-interactive login only in conjunction with -u. Default: prompt")
                .Callback(password => _password = password);
            
            return base.ParseArgs(args);
        }

        public override async Task RunAsync()
        {
            if (string.IsNullOrWhiteSpace(_username))
                await _armManager.LoginAsync();
            else
            {
                if (string.IsNullOrWhiteSpace(_password))
                    _password = PromptForPassword();

                await _armManager.LoginAsync(_username, _password);
            }

            await PrintAccountsAsync();
        }

        private static string PromptForPassword()
        {
            Console.Write("password: ");
            return SecurityHelpers.ReadPassword();
        }
    }
}
