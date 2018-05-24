using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Core.Setup;
using System;
using System.Reflection;

namespace SyboChallenge.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddBackgroundJob<TService>(this IServiceCollection services)
            where TService : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<TService>();
            return services;
        }

        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), lifetime));
            return services;
        }

        public static IServiceCollection AddStore<TStore, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TStore : class
            where TImplementation : class, TStore
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Add(ServiceDescriptor.Describe(typeof(TStore), typeof(TImplementation), lifetime));
            return services;
        }

        public static IServiceCollection AddValidator<TModel, TValidator>(this IServiceCollection services)
            where TModel : class
            where TValidator : class, IValidator<TModel>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddSingleton<IValidator<TModel>, TValidator>();
        }

        public static IServiceCollection AddValidatorsFrom(this IServiceCollection services, Assembly assembly)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            AssemblyScanner.FindValidatorsInAssembly(assembly).ForEach(result =>
            {
                services.Add(ServiceDescriptor.Singleton(result.InterfaceType, result.ValidatorType));
            });

            return services;
        }

        public static IServiceCollection AddInstaller(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddTransient<Installer>();
        }

        public static IServiceCollection AddInstallerStep<TInstallerStep>(this IServiceCollection services)
            where TInstallerStep : class, IInstallStep
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddTransient<IInstallStep, TInstallerStep>();
        }

        public static IServiceCollection AddInstallerStep<TInstallerStep>(this IServiceCollection services, Func<IServiceProvider, TInstallerStep> factory)
            where TInstallerStep : class, IInstallStep
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return services.AddTransient<IInstallStep, TInstallerStep>(factory);
        }
    }
}
