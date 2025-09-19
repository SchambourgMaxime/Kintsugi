#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UIToolkitParticles
{
    public static class PackageImporter
    {
        public enum PackageType
        {
            BuiltIn = 0
        }

        private class Package
        {
            public PackageType PackageType;
            public string PackagePath;

            public Package(PackageType packageType, string packagePath)
            {
                PackageType = packageType;
                PackagePath = packagePath;
            }
        }

        static List<Package> Packages = new List<Package>()
        {
        };

        static Package getPackageFor(PackageType packageType)
        {
            foreach (var pkg in Packages)
            {
                if (pkg.PackageType == packageType)
                    return pkg;
            }

            return null;
        }

        static System.Action _onComplete;

        #region Start Import Delayed
        static double startPackageImportAt;

        public static void ImportDelayed(System.Action onComplete)
        {
            // Some assets may not be loaded at this time. Thus we wait for them to be imported.
            _onComplete = onComplete;
            EditorApplication.update -= onEditorUpdate;
            EditorApplication.update += onEditorUpdate;
            startPackageImportAt = EditorApplication.timeSinceStartup + 3; // wait N seconds
        }

        static void onEditorUpdate()
        {
            // wait for the time to reach startPackageImportAt
            if (startPackageImportAt - EditorApplication.timeSinceStartup < 0)
            {
                EditorApplication.update -= onEditorUpdate;
                ImportPackages();
                return;
            }
        }
        #endregion

        static int _crossCompileCallbackID = -1;

        const string PackagesToImportKey = "Kamgam.UIToolkitParticles.PackagesToImport";

        [MenuItem("Tools/UI Toolkit Particles/Debug/Import packages", priority = 500)]
        public static void ImportPackages()
        {
            // Don't import during play mode.
            if (EditorApplication.isPlaying)
                return;

            Debug.Log("PackageImporter: Importing..");

            var packagesToImport = initializePackagesToImport();
            startImportingNextPackage(packagesToImport);
        }

        static List<PackageType> initializePackagesToImport()
        {
            var packages = new List<PackageType>();
            addPackageIfNeeded(packages);

            setPackagesToImportList(packages);

            if (packages.Count == 0)
            {
                Debug.Log("PackageImporter: Everything seems okay, no more imports needed.");
                onPackageImportDone(_onComplete);
            }

            return packages;
        }

        static void addPackageIfNeeded(List<PackageType> packages)
        {
            /*
             * TODO
            if (false)
            {
                Debug.Log("PackageImporter: todo");
            }
            */
        }

        static void startImportingNextPackage(List<PackageType> packagesToImport)
        {
            if (packagesToImport.Count > 0)
            {
                var package = getPackageFor(packagesToImport[0]);
                removePackageToImportList(package.PackageType);
                startImportingPackage(package);
            }
        }

        static void startImportingPackage(Package package)
        {
            // AssetDatabase.importPackageCompleted callbacks are lost after a recompile.
            // Therefore, if the package includes any scripts then these will not be called.
            // See: https://forum.unity.com/threads/assetdatabase-importpackage-callbacks-dont-work.544031/#post-3716791

            // We use CrossCompileCallbacks to register a callback for after compilation.
            _crossCompileCallbackID = CrossCompileCallbacks.RegisterCallback(onPackageImportedAfterRecompile);
            // We also have to store the external callback (if there is one)
            CrossCompileCallbacks.StoreAction(typeof(PackageImporter).FullName + ".importedCallack", _onComplete);
            // Delay to avoid "Calling ... from assembly reloading callbacks are not supported." errors.
            CrossCompileCallbacks.DelayExecutionAfterCompilation = true;

            // If the package does not contain any scripts the we can still use the normal callbacks.
            AssetDatabase.importPackageCompleted -= onPackageImported;
            AssetDatabase.importPackageCompleted += onPackageImported;

            // import package
            Debug.Log("PackageImporter: Importing '" + package.PackagePath + "'.");
            AssetDatabase.ImportPackage(package.PackagePath, interactive: false);
            AssetDatabase.SaveAssets();
        }

        static void setPackagesToImportList(List<PackageType> packages)
        {
            var packagesAsInts = packages.Select(p => (int)p).ToArray();
            SessionState.SetIntArray(PackagesToImportKey, packagesAsInts);
        }

        static List<PackageType> getPackagesToImportList()
        {
            var packagesAsInts = SessionState.GetIntArray(PackagesToImportKey, new int[] { });
            var packages = packagesAsInts.Select(p => (PackageType)p).ToList();
            return packages;
        }

        static void removePackageToImportList(PackageType package)
        {
            var packages = getPackagesToImportList();
            packages.Remove(package);
            setPackagesToImportList(packages);
        }

        // This is only execute if the package did not contain any script files.
        static void onPackageImported(string packageName)
        {
            Debug.Log("PackageImporter: Package '" + packageName + "' imported.");

            // There was no recompile. Thus we clear the registered callback.
            CrossCompileCallbacks.ReleaseIndex(_crossCompileCallbackID);

            // Check if it is one of our packages.
            // Abort if not.
            bool isFixerPackage = false;
            foreach (var pkg in Packages)
            {
                if (pkg.PackagePath.Contains(packageName))
                    isFixerPackage = true;
            }
            if (!isFixerPackage)
                return;

            AssetDatabase.importPackageCompleted -= onPackageImported;

            onPackageImportDone(_onComplete);
            _onComplete = null;
        }

        static void onPackageImportedAfterRecompile()
        {
            Debug.Log("PackageImporter: Recompile detected. Assuming package import is done.");

            // The registered callback is already cleared by now.
            // Now we let's retrieve that stored extenal callback and hand it over.
            var onComplete = CrossCompileCallbacks.GetStoredAction(typeof(PackageImporter).FullName + ".importedCallack");
            onPackageImportDone(onComplete);
        }

        static void onPackageImportDone(System.Action onComplete)
        {
            // Check for more packages to import
            Debug.Log("PackageImporter: package imported. Looking for next package.");

            var packagesToImport = getPackagesToImportList();

            if (packagesToImport.Count > 0)
            {
                // Make sure the onComplete callback is retained across multiple package loads.
                _onComplete = onComplete;

                // Start importing
                startImportingNextPackage(packagesToImport);
            }
            else
            {
                AssetDatabase.SaveAssets();
                onComplete?.Invoke();

                Debug.Log("PackageImporter: Done (no more packages to import).");
            }
        }
    }
}
#endif