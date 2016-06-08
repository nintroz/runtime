﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using Microsoft.DotNet.CoreSetup.Test;

namespace Microsoft.DotNet.CoreSetup.Test.HostActivation.PortableApp
{
    public class GivenThatICareAboutPortableAppActivation
    {
        private static TestProjectFixture PreviouslyBuiltAndRestoredPortableTestProjectFixture { get; set; }
        private static TestProjectFixture PreviouslyPublishedAndRestoredPortableTestProjectFixture { get; set; }
        private static RepoDirectoriesProvider RepoDirectories { get; set; }

        static GivenThatICareAboutPortableAppActivation()
        {
            RepoDirectories = new RepoDirectoriesProvider();

            PreviouslyBuiltAndRestoredPortableTestProjectFixture = new TestProjectFixture("PortableApp", RepoDirectories)
                .EnsureRestored(RepoDirectories.CorehostPackages, RepoDirectories.CorehostDummyPackages)
                .BuildProject();

            PreviouslyPublishedAndRestoredPortableTestProjectFixture = new TestProjectFixture("PortableApp", RepoDirectories)
                .EnsureRestored(RepoDirectories.CorehostPackages, RepoDirectories.CorehostDummyPackages)
                .PublishProject();
        }

        [Fact]
        public void Muxer_activation_of_Build_Output_Portable_DLL_with_DepsJson_and_RuntimeConfig_Local_Succeeds()
        {
            var fixture = PreviouslyBuiltAndRestoredPortableTestProjectFixture
                .Copy();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;

            dotnet.Exec(appDll).CaptureStdErr().CaptureStdOut().Execute().Should().Pass();
            dotnet.Exec("exec", appDll).CaptureStdErr().CaptureStdOut().Execute().Should().Pass();
        }

        
        [Fact]
        public void Muxer_Exec_activation_of_Build_Output_Portable_DLL_with_DepsJson_Local_and_RuntimeConfig_Remote_Without_AdditionalProbingPath_Fails()
        {
            var fixture = PreviouslyBuiltAndRestoredPortableTestProjectFixture
                .Copy()
                .MoveRuntimeConfigToSubdirectory();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;
            var runtimeConfig = fixture.TestProject.RuntimeConfigJson;
            
            dotnet.Exec("exec", "--runtimeconfig", runtimeConfig, appDll)
                .CaptureStdErr()
                .CaptureStdOut()
                .Execute()
                .Should()
                .Fail();
        }

        [Fact]
        public void Muxer_Exec_activation_of_Build_Output_Portable_DLL_with_DepsJson_Local_and_RuntimeConfig_Remote_With_AdditionalProbingPath_Succeeds()
        {
            var fixture = PreviouslyBuiltAndRestoredPortableTestProjectFixture
                .Copy()
                .MoveRuntimeConfigToSubdirectory();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;
            var runtimeConfig = fixture.TestProject.RuntimeConfigJson;
            var additionalProbingPath = RepoDirectories.NugetPackages;

            dotnet.Exec(
                    "exec", 
                    "--runtimeconfig", runtimeConfig, 
                    "--additionalprobingpath", additionalProbingPath,
                    appDll)
                .CaptureStdErr().CaptureStdOut().Execute().Should().Pass();
        }

        [Fact]
        public void Muxer_Exec_activation_of_Build_Output_Portable_DLL_with_DepsJson_Remote_and_RuntimeConfig_Local_Succeeds()
        {
            var fixture = PreviouslyBuiltAndRestoredPortableTestProjectFixture
                 .Copy()
                 .MoveDepsJsonToSubdirectory();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;
            var depsJson = fixture.TestProject.DepsJson;

            dotnet.Exec("exec", "--depsfile", depsJson, appDll)
                .CaptureStdErr()
                .CaptureStdOut()
                .Execute()
                .Should()
                .Pass();
        }

        [Fact]
        public void Muxer_activation_of_Publish_Output_Portable_DLL_with_DepsJson_and_RuntimeConfig_Local_Succeeds()
        {
            var fixture = PreviouslyPublishedAndRestoredPortableTestProjectFixture
                .Copy();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;

            dotnet.Exec(appDll).CaptureStdErr().CaptureStdOut().Execute().Should().Pass();
            dotnet.Exec("exec", appDll).CaptureStdErr().CaptureStdOut().Execute().Should().Pass();
        }


        [Fact]
        public void Muxer_Exec_activation_of_Publish_Output_Portable_DLL_with_DepsJson_Local_and_RuntimeConfig_Remote_Succeeds()
        {
            var fixture = PreviouslyPublishedAndRestoredPortableTestProjectFixture
                .Copy()
                .MoveRuntimeConfigToSubdirectory();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;
            var runtimeConfig = fixture.TestProject.RuntimeConfigJson;

            dotnet.Exec("exec", "--runtimeconfig", runtimeConfig, appDll)
                .CaptureStdErr()
                .CaptureStdOut()
                .Execute()
                .Should()
                .Pass();
        }

        [Fact]
        public void Muxer_Exec_activation_of_Publish_Output_Portable_DLL_with_DepsJson_Remote_and_RuntimeConfig_Local_Fails()
        {
            var fixture = PreviouslyPublishedAndRestoredPortableTestProjectFixture
                 .Copy()
                 .MoveDepsJsonToSubdirectory();

            var dotnet = fixture.BuiltDotnet;
            var appDll = fixture.TestProject.AppDll;
            var depsJson = fixture.TestProject.DepsJson;

            dotnet.Exec("exec", "--depsfile", depsJson, appDll)
                .CaptureStdErr()
                .CaptureStdOut()
                .Execute()
                .Should()
                .Fail();
        }
    }
}
