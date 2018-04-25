﻿// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arctium.Core.Configuration;
using Arctium.Core.Database;
using Lappa.ORM.Constants;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Arctium.API.Misc
{
    public class ApiConfig : ConfigBase<ApiConfig>
    {
        [ConfigEntry("API.Bind.Host", "127.0.0.1")]
        public static string BindHost { get; }

        [ConfigEntry("API.Bind.Port", 5543)]
        public static int BindPort { get; }

        [ConfigEntry("API.Auth", false)]
        public static bool Auth { get; }

        [ConfigEntry("API.Auth.Type", "")]
        public static string AuthType { get; }

        [ConfigEntry("API.Protocol", HttpProtocols.Http2)]
        public static HttpProtocols Protocol { get; }

        [ConfigEntry("API.Tls", false)]
        public static bool Tls { get; }

        [ConfigEntry("API.Tls.Certificate", "")]
        public static string TlsCertificate { get; }

        [ConfigEntry("Database.Type", DatabaseType.MySql)]
        public static DatabaseType DatabaseType { get; }

        [ConfigEntry("Database.Aurora", default(DatabaseSettings))]
        public static DatabaseSettings AuroraDatabase { get; }
    }
}
