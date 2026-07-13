// Copyright (C) Ascensio System SIA, 2009-2026
// 
// This program is a free software product. You can redistribute it and/or
// modify it under the terms of the GNU Affero General Public License (AGPL)
// version 3 as published by the Free Software Foundation, together with the
// additional terms provided in the LICENSE file.
// 
// This program is distributed WITHOUT ANY WARRANTY, without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. For
// details, see the GNU AGPL at: https://www.gnu.org/licenses/agpl-3.0.html
// 
// You can contact Maticon Office LLC by email at info@maticonoffice.ru
// or by postal mail at Office 1840, Premises 4/45, 12 Presnenskaya Embankment, Moscow, 123112, Russia,
// Office 1840, Premises 4/45, 12 Presnenskaya Embankment, Moscow, 123112, Russia.
// 
// The interactive user interfaces in modified versions of the Program
// are required to display Appropriate Legal Notices in accordance with
// Section 5 of the GNU AGPL version 3.
// 
// No trademark rights are granted under this License.
// 
// All non-code elements of the Product, including illustrations,
// icon sets, and technical writing content, are licensed under the
// Creative Commons Attribution-ShareAlike 4.0 International License:
// https://creativecommons.org/licenses/by-sa/4.0/legalcode
// 
// This license applies only to such non-code elements and does not
// modify or replace the licensing terms applicable to the Program's
// source code, which remains licensed under the GNU Affero General
// Public License v3.
// 
// SPDX-License-Identifier: AGPL-3.0-only

using System.Threading.Channels;

namespace ASC.Core.Common.Notify.Engine;

[Singleton]
public class NotifySenderService(
    NotifyEngine notifyEngine,
    ChannelReader<NotifyRequest> channelReader,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<NotifySenderService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in channelReader.ReadAllAsync(stoppingToken))
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            foreach (var action in notifyEngine.Actions)
            {
                ((INotifyEngineAction)scope.ServiceProvider.GetRequiredService(action)).AfterTransferRequest(request);
            }

            try
            {
                await notifyEngine.SendNotify(request, scope);
            }
            catch (Exception e)
            {
                logger.ErrorSendNotify(e);
            }
        }
    }
}

public static class NotifySenderServiceConfigurator
{
    public static void ConfigureNotificationServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<NotifyRequest>());
        services.AddSingleton(svc => svc.GetRequiredService<Channel<NotifyRequest>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<NotifyRequest>>().Writer);
        services.AddHostedService<NotifySenderService>();
    }
}