using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitVersion] readonly GitVersion GitVersion;


    Target GetSemVer => _ => _
        .Executes(() =>
        {
            Log.Information("GitVersion = {Value}", GitVersion.AssemblySemVer);
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild();
        });

    static readonly string publishFolder = RootDirectory / "publish";

    Target Publish => _ => _
        .DependsOn(Compile)
        .DependsOn(GetSemVer)
        .Executes(() =>
        {
            DotNetTasks.DotNetPublish(s =>
                s.SetOutput(publishFolder)
                    .SetAssemblyVersion(GitVersion.AssemblySemVer)
            );
        });
}