﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Simplicity.AspNetCore.Identity;
using Simplicity.AspNetCore.Identity.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class StudensIdentityEntityFrameworkBuilderExtensions
{
    /// <summary>
    /// Adds an extened Studens Entity Framework implementation of identity information stores.
    /// </summary>
    /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
    /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
    /// TODO: Check the RoleStore and UserOnlyStore ?
    public static IdentityBuilder AddStudensEntityFrameworkStores<TContext>(this IdentityBuilder builder)
        where TContext : DbContext
    {
        AddIdentityStores(builder.Services, builder.UserType, builder.RoleType, typeof(TContext));
        return builder;
    }

    private static void AddIdentityStores(IServiceCollection services, Type userType, Type roleType, Type contextType)
    {
        var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
        if (identityUserType == null)
        {
            throw new InvalidOperationException("Resources.NotIdentityUser");
        }

        var keyType = identityUserType.GenericTypeArguments[0];

        if (roleType != null)
        {
            var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
            if (identityRoleType == null)
            {
                throw new InvalidOperationException("Resources.NotIdentityRole");
            }

            Type? userStoreType = null;
            Type? roleStoreType = null;
            var identityContext = FindGenericBaseType(contextType, typeof(IdentityDbContext<,,,,,,,>));
            if (identityContext == null)
            {
                // If its a custom DbContext, we can only add the default POCOs
                userStoreType = typeof(IdentityUserStore<,,,>).MakeGenericType(userType, roleType, contextType, keyType);
                roleStoreType = typeof(IdentityRoleStore<,,>).MakeGenericType(roleType, contextType, keyType);
            }
            else
            {
                userStoreType = typeof(IdentityUserStore<,,,,,,,,>).MakeGenericType(userType, roleType, contextType,
                    identityContext.GenericTypeArguments[2],
                    identityContext.GenericTypeArguments[3],
                    identityContext.GenericTypeArguments[4],
                    identityContext.GenericTypeArguments[5],
                    identityContext.GenericTypeArguments[7],
                    identityContext.GenericTypeArguments[6]);

                roleStoreType = typeof(IdentityRoleStore<,,,,>).MakeGenericType(roleType, contextType,
                    identityContext.GenericTypeArguments[2],
                    identityContext.GenericTypeArguments[4],
                    identityContext.GenericTypeArguments[6]);
            }
            services.TryAddScoped(typeof(IIdentityUserStore<>).MakeGenericType(userType), userStoreType);
            services.TryAddScoped(typeof(IIdentityRoleStore<>).MakeGenericType(roleType), roleStoreType);
        }
        else
        {   // No Roles
            Type? userStoreType = null;
            var identityContext = FindGenericBaseType(contextType, typeof(IdentityUserContext<,,,,>));
            if (identityContext == null)
            {
                // If its a custom DbContext, we can only add the default POCOs
                userStoreType = typeof(IdentityUserOnlyStore<,,>).MakeGenericType(userType, contextType, keyType);
            }
            else
            {
                userStoreType = typeof(IdentityUserOnlyStore<,,,,,>).MakeGenericType(userType, contextType,
                    identityContext.GenericTypeArguments[1],
                    identityContext.GenericTypeArguments[2],
                    identityContext.GenericTypeArguments[3],
                    identityContext.GenericTypeArguments[4]);
            }
            services.TryAddScoped(typeof(IIdentityUserStore<>).MakeGenericType(userType), userStoreType);
        }
    }

    private static Type FindGenericBaseType(Type currentType, Type genericBaseType)
    {
        var type = currentType;
        while (type != null)
        {
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericType != null && genericType == genericBaseType)
            {
                return type;
            }
            type = type.BaseType;
        }
        return null;
    }
}