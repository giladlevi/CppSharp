// ----------------------------------------------------------------------------
// <auto-generated>
// This is autogenerated code by CppSharp.
// Do not edit this file or all your changes will be lost after re-generation.
// </auto-generated>
// ----------------------------------------------------------------------------
#pragma once

#include "CppSharp.h"
#include <CLI.h>

namespace CLI
{
    ref class Date;
    ref class TestProtectedDestructors;
    ref class Types;
}

namespace CLI
{
    public ref class Types : ICppInstance
    {
    public:

        property ::Types* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        Types(::Types* native);
        static Types^ __CreateInstance(::System::IntPtr native);
        Types();

        Types(::CLI::Types^ _0);

        ~Types();

        !Types();

        int AttributedSum(int _0, int _1);

        virtual bool Equals(System::Object^ object) override;

        virtual int GetHashCode() override;

        protected:
        bool __ownsNativeInstance;
    };

    public ref class TestProtectedDestructors : ICppInstance
    {
    public:

        property ::TestProtectedDestructors* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        TestProtectedDestructors(::TestProtectedDestructors* native);
        static TestProtectedDestructors^ __CreateInstance(::System::IntPtr native);
        TestProtectedDestructors();

        TestProtectedDestructors(::CLI::TestProtectedDestructors^ _0);

        virtual bool Equals(System::Object^ object) override;

        virtual int GetHashCode() override;

        protected:
        bool __ownsNativeInstance;
    };

    public ref class Date : ICppInstance
    {
    public:

        property ::Date* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        Date(::Date* native);
        static Date^ __CreateInstance(::System::IntPtr native);
        Date(int m, int d, int y);

        Date(::CLI::Date^ _0);

        ~Date();

        !Date();

        property int Mo
        {
            int get();
            void set(int);
        }

        property int Da
        {
            int get();
            void set(int);
        }

        property int Yr
        {
            int get();
            void set(int);
        }

        System::String^ TestStdString(System::String^ s);

        virtual System::String^ ToString() override;

        virtual bool Equals(System::Object^ object) override;

        virtual int GetHashCode() override;

        protected:
        bool __ownsNativeInstance;
    };

    public ref class CLI
    {
    public:
        static void TestFreeFunction();
    };
}