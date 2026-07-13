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

namespace ASC.Core;

public interface ITenantService
{
    Task<byte[]> GetTenantSettingsAsync(int tenant, string key);
    byte[] GetTenantSettings(int tenant, string key);
    Task<IEnumerable<Tenant>> GetTenantsAsync(DateTime from, bool active = true);
    Task<IEnumerable<Tenant>> GetTenantsAsync(List<int> ids);
    Task<IEnumerable<Tenant>> GetTenantsAsync(string login, string passwordHash);
    Task<IEnumerable<TenantVersion>> GetTenantVersionsAsync();
    Task<Tenant> GetTenantAsync(int id);
    Task<Tenant> RestoreTenantAsync(Tenant oldTenant, Tenant newTenant, CoreSettings coreSettings);
    Task<Tenant> GetTenantAsync(string domain);
    Tenant GetTenant(string domain);
    Tenant GetTenantForStandaloneWithoutAlias(string ip);
    Task<Tenant> GetTenantForStandaloneWithoutAliasAsync(string ip);
    Task<Tenant> SaveTenantAsync(CoreSettings coreSettings, Tenant tenant);
    Task RemoveTenantAsync(Tenant tenant, bool auto = false);
    Task SetTenantSettingsAsync(int tenant, string key, byte[] data);
    Task PermanentlyRemoveTenantAsync(int id);
    Task ValidateDomainAsync(string domain);
    Task<bool> IsForbiddenDomainAsync(string domain);
    void ValidateTenantName(string name);
}