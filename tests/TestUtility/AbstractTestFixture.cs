﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TestUtility.Logging;
using Xunit;
using Xunit.Abstractions;

namespace TestUtility
{
    public class SharedOmniSharpHostFixture : IDisposable
    {
        public SharedOmniSharpHostFixture()
        {
        }

        public void Dispose()
        {
            OmniSharpTestHost?.Dispose();
        }

        public OmniSharpTestHost OmniSharpTestHost { get; set; }

    }

    public abstract class AbstractTestFixture : IClassFixture<SharedOmniSharpHostFixture>
    {
        protected OmniSharpTestHost OmniSharpTestHost { get; }
        protected readonly ITestOutputHelper TestOutput;
        protected readonly ILoggerFactory LoggerFactory;

        protected AbstractTestFixture(ITestOutputHelper output)
        {
            TestOutput = output;
            LoggerFactory = new LoggerFactory()
                .AddXunit(output);
        }

        protected AbstractTestFixture(ITestOutputHelper output, SharedOmniSharpHostFixture sharedOmniSharpHostFixture)
        {
            TestOutput = output;
            LoggerFactory = new LoggerFactory()
                .AddXunit(output);

            if (sharedOmniSharpHostFixture.OmniSharpTestHost == null)
            {
                sharedOmniSharpHostFixture.OmniSharpTestHost = CreateOmniSharpHost();
            }
            else
            {
                sharedOmniSharpHostFixture.OmniSharpTestHost.ClearWorkspace();
            }

            OmniSharpTestHost = sharedOmniSharpHostFixture.OmniSharpTestHost;
        }

        protected OmniSharpTestHost CreateEmptyOmniSharpHost()
        {
            var host = OmniSharpTestHost.Create(path: null, testOutput: this.TestOutput);
            host.AddFilesToWorkspace();
            return host;
        }

        protected OmniSharpTestHost CreateOmniSharpHost(string path = null, IEnumerable<KeyValuePair<string, string>> configurationData = null, DotNetCliVersion dotNetCliVersion = DotNetCliVersion.Current) =>
            OmniSharpTestHost.Create(path, this.TestOutput, configurationData, dotNetCliVersion);

        protected OmniSharpTestHost CreateOmniSharpHost(params TestFile[] testFiles) => 
            CreateOmniSharpHost(testFiles, null);

        protected OmniSharpTestHost CreateOmniSharpHost(TestFile[] testFiles, IEnumerable<KeyValuePair<string, string>> configurationData)
        {
            var host = OmniSharpTestHost.Create(path: null, testOutput: this.TestOutput, configurationData: configurationData);

            if (testFiles.Length > 0)
            {
                host.AddFilesToWorkspace(testFiles);
            }

            return host;
        }
    }
}
