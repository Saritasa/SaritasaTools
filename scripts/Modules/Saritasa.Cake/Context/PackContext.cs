﻿using Cake.Common;
using Cake.Core;

namespace Saritasa.Cake.Context;

/// <summary>
/// Build context.
/// </summary>
public class PackContext : CommonContext
{
    /// <summary>
    /// MsBuild Configuration.
    /// </summary>
    /// <remarks>DefaultValue - Release.
    /// Was override if invoke "Saritasa.Cake.ps1" with "configuration" argument (./Saritasa.Cake.ps1 --configuration Debug).</remarks>
    public string MsBuildConfiguration { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">Cake context.</param>
    public PackContext(ICakeContext context) : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Release");
    }
}
