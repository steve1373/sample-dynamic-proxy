using DynamicProxySample1.Domain;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DynamicProxySample1
{
    public class DynamicProxyCreator
    {
        public static TTargetType MakeProxy<TTargetType>()
        {
            Type targetType = CreateProxyType(typeof(TTargetType));
            return (TTargetType)Activator.CreateInstance(targetType, null);
        }

        private static Type CreateProxyType(Type baseType)
        {
            List<String> referenceList = new List<String>(); // store a list of necessary references
            System.Reflection.Assembly assembly = baseType.Assembly;
            
            CodeCompileUnit ccu = new CodeCompileUnit();
            // create the 
            string defaultNamespace = "DynamicProxySample1.Domain.Proxy";
            CodeNamespace myNamespace = new CodeNamespace(defaultNamespace);

            foreach (Module m in assembly.GetLoadedModules(false))
            {
                String subAssemblyName = m.Assembly.GetName().Name;
                myNamespace.Imports.Add(new CodeNamespaceImport(subAssemblyName));
                referenceList.Add(m.ToString());
            }

            myNamespace.Imports.Add(new CodeNamespaceImport("DynamicProxySample1"));
            myNamespace.Imports.Add(new CodeNamespaceImport("DynamicProxySample1.Domain"));
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration(baseType.Name + "Proxy");
            CodeTypeReference baseClassTypeReference = new CodeTypeReference(baseType.FullName);

            targetClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            targetClass.IsClass = true;
            targetClass.BaseTypes.Add(baseClassTypeReference);

            // Add the additional interface - IReservable
            CodeTypeReference ireserveType = new CodeTypeReference();
            ireserveType.BaseType = typeof(IReservable).FullName;
            targetClass.BaseTypes.Add(ireserveType);

            // This will use the CodeDOM to add the interface to this new subclass of the original target class.
            addIReservable(targetClass);

            // Other additional subclass operations can go here.
            // Most common: method overrides, property overrides.

            myNamespace.Types.Add(targetClass);
            ccu.Namespaces.Add(myNamespace);

            // Now we can build the class and then compile it ... 
            string fileNameToUse = "c:\\temp\\proxy_" + baseType.FullName + "Proxy.cs";
            GenerateCSharpCode(ccu, fileNameToUse);

            Assembly newAssembly = CompileCSharpCode(fileNameToUse, referenceList.ToArray());
            if (assembly != null)
            {
                Type t = newAssembly.GetType(defaultNamespace + "." + targetClass.Name);
                if (t != null)
                {
                    return t;
                }
            }

            // This is technically an error; return the baseType if we were not able to 
            // create the proxy.
            return baseType;
        }

        /**
         * Add the IReservable interface implementation
         */
        private static void addIReservable(CodeTypeDeclaration targetClass)
        {
            CodeMemberMethod checkout = new CodeMemberMethod();
            checkout.Attributes = MemberAttributes.Public;

            checkout.Name = "checkout";
            checkout.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Student), "student"));
            checkout.ReturnType = new CodeTypeReference(typeof(bool));
            targetClass.Members.Add(checkout);

            // ReservationTracker rt = ReservationTracker.getInstance();

            CodeMethodInvokeExpression rtGetInstance = new CodeMethodInvokeExpression();
            rtGetInstance.Method =
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(ReservationTracker)),
                    "getInstance");
            CodeVariableDeclarationStatement reservationTrackerDecl =
                new CodeVariableDeclarationStatement(typeof(ReservationTracker), "reservationTracker", rtGetInstance);
            
            checkout.Statements.Add(reservationTrackerDecl);

            // Add the method invocation
            CodeMethodInvokeExpression callReserve = new CodeMethodInvokeExpression();
            callReserve.Method.TargetObject = new CodeVariableReferenceExpression("reservationTracker");
            callReserve.Method.MethodName = "reserve";
            callReserve.Parameters.Add(new CodeThisReferenceExpression());
            callReserve.Parameters.Add(new CodeVariableReferenceExpression("student"));
            checkout.Statements.Add(callReserve);

            // Return the result.
            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(
                new CodeSnippetExpression("true"));
            checkout.Statements.Add(returnStatement);
        }

        /**
         * Generate the source code document from the model.
         */
        internal static void GenerateCSharpCode(CodeCompileUnit targetUnit, string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }

        /**
         * Compile the generated code.
         */
        public static Assembly CompileCSharpCode(string sourceFile, string[] necessaryReferenceList)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();

            string libDir = AppDomain.CurrentDomain.RelativeSearchPath;

            // Build the parameters for source compilation.
            CompilerParameters cp = new CompilerParameters();
            cp.IncludeDebugInformation = true;
            cp.TreatWarningsAsErrors = false;

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");
            if (necessaryReferenceList != null)
            {
                foreach (string s in necessaryReferenceList)
                {
                    cp.ReferencedAssemblies.Add(s);
                }
            }

            // This may be required for ASP.NET Web API ... 
            if (libDir != null)
            {
                cp.CompilerOptions += string.Format("/lib:{0}", libDir);
            }
           
            // Generate an executable instead of 
            // a class library.
            cp.GenerateExecutable = false;

            // Set the assembly file name to generate.
            // cp.OutputAssembly = assemblyFile;

            // Save the assembly as a physical file.
            cp.GenerateInMemory = true;

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",
                    sourceFile, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.",
                    sourceFile, cr.PathToAssembly);
            }

            // Return the results of compilation. 
            if (cr.Errors.Count > 0)
            {
                return null;
            }
            else
            {
                return cr.CompiledAssembly;
            }
        }
    }
}
