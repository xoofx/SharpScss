// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpScss
{
    /// <summary>
    /// Native wrapper around libsass. This is a copy of all ADDAPI/ADDCALL from libsass.
    /// The API was then transformed for C#
    /// - char* are using StringUtf8
    /// - All struct* are using opaque pointer struct 
    /// 
    /// Note that some APIs are currently commented (custom Functions/Values) as they are not exposed.
    /// </summary>
    internal static unsafe class LibSass
    {
        private const string LibSassDll = "libsass";

#if !CORE
        static LibSass()
        {
            // This code is only working on Windows NET2-NET4.5 platforms. We preload the assembly depending on the x86/x64 platform
            // so that further DllImport in this class will work automatically
            var directory = Path.GetDirectoryName(typeof (LibSass).Assembly.Location);
            var loadPath = Path.Combine(directory, (IntPtr.Size == 8 ? @"x64\" : @"x86\") + LibSassDll + ".dll");
            LoadLibrary(loadPath);
        }
#endif

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        // libsass\include\sass\base.h (4 hits)
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern IntPtr sass_string_alloc(IntPtr size);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_string_quote(StringUtf8 str, char quote_mark);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_string_unquote(StringUtf8 str);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_resolve_file(StringUtf8 path, StringUtf8* incs);       
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 libsass_version();

        // libsass\include\sass\context.h (82 hits)
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Options sass_make_options();
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_File_Context sass_make_file_context(StringUtf8 input_path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Data_Context sass_make_data_context(StringUtf8 source_string);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_compile_file_context(Sass_File_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_compile_data_context(Sass_Data_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Compiler sass_make_file_compiler(Sass_File_Context file_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Compiler sass_make_data_compiler(Sass_Data_Context data_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_compiler_parse(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_compiler_execute(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_compiler(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_file_context(Sass_File_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_data_context(Sass_Data_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Context sass_file_context_get_context(Sass_File_Context file_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Context sass_data_context_get_context(Sass_Data_Context data_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Options sass_context_get_options(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Options sass_file_context_get_options(Sass_File_Context file_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Options sass_data_context_get_options(Sass_Data_Context data_ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_file_context_set_options(Sass_File_Context file_ctx, Sass_Options opt);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_data_context_set_options(Sass_Data_Context data_ctx, Sass_Options opt);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_option_get_precision(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern ScssOutputStyle sass_option_get_output_style(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_option_get_source_comments(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_option_get_source_map_embed(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_option_get_source_map_contents(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_option_get_omit_source_map_url(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_option_get_is_indented_syntax_src(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_indent(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_linefeed(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_input_path(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_output_path(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_plugin_path(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_include_path(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_source_map_file(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_option_get_source_map_root(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_List sass_option_get_c_headers(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_List sass_option_get_c_importers(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Function_List sass_option_get_c_functions(Sass_Options options);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_precision(Sass_Options options, int precision);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_output_style(Sass_Options options, ScssOutputStyle output_style);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_source_comments(Sass_Options options, bool source_comments);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_source_map_embed(Sass_Options options, bool source_map_embed);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_source_map_contents(Sass_Options options, bool source_map_contents);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_omit_source_map_url(Sass_Options options, bool omit_source_map_url);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_is_indented_syntax_src(Sass_Options options,
            bool is_indented_syntax_src);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_indent(Sass_Options options, StringUtf8 indent);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_linefeed(Sass_Options options, StringUtf8 linefeed);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_input_path(Sass_Options options, StringUtf8 input_path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_output_path(Sass_Options options, StringUtf8 output_path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_plugin_path(Sass_Options options, StringUtf8 plugin_path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_include_path(Sass_Options options, StringUtf8 include_path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_source_map_file(Sass_Options options, StringUtf8 source_map_file);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_source_map_root(Sass_Options options, StringUtf8 source_map_root);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_c_headers(Sass_Options options, Sass_Importer_List c_headers);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_c_importers(Sass_Options options, Sass_Importer_List c_importers);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_set_c_functions(Sass_Options options, Sass_Function_List c_functions);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_output_string(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern int sass_context_get_error_status(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_error_json(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_error_text(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_error_message(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_error_file(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_error_src(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_context_get_error_line(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_context_get_error_column(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_get_source_map_string(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8* sass_context_get_included_files(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_context_get_included_files_size(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_error_json(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_error_text(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_error_message(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_error_file(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_output_string(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_context_take_source_map_string(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8* sass_context_take_included_files(Sass_Context ctx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Compiler_State sass_compiler_get_state(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Context sass_compiler_get_context(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Options sass_compiler_get_options(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_compiler_get_import_stack_size(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_compiler_get_last_import(Sass_Compiler compiler);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_compiler_get_import_entry(Sass_Compiler compiler, size_t idx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_push_plugin_path(Sass_Options options, StringUtf8 path);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_option_push_include_path(Sass_Options options, StringUtf8 path);

        // libsass\include\sass\functions.h (32 hits)
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_List sass_make_importer_list(size_t length);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_Entry sass_importer_get_list_entry(Sass_Importer_List list, size_t idx);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_importer_set_list_entry(Sass_Importer_List list, size_t idx,
            Sass_Importer_Entry entry);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_Entry sass_make_importer(Sass_Importer_Fn importer, double priority,
            void* cookie);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Importer_Fn sass_importer_get_function(Sass_Importer_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_importer_get_priority(Sass_Importer_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void* sass_importer_get_cookie(Sass_Importer_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_importer(Sass_Importer_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_List sass_make_import_list(size_t length);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_make_import_entry(StringUtf8 path, StringUtf8 source, StringUtf8 srcmap);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_make_import(StringUtf8 imp_path, StringUtf8 abs_base, StringUtf8 source,
            StringUtf8 srcmap);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_import_set_error(Sass_Import_Entry import, StringUtf8 message, size_t line,
            size_t col);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_import_set_list_entry(Sass_Import_List list, size_t idx, Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Import_Entry sass_import_get_list_entry(Sass_Import_List list, size_t idx);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_get_imp_path(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_get_abs_path(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_get_source(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_get_srcmap(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_take_source(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_take_srcmap(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_import_get_error_line(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_import_get_error_column(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_import_get_error_message(Sass_Import_Entry entry);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_import_list(Sass_Import_List importList);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_import(Sass_Import_Entry entry);

        /*
         * NOTE: we don't support custom functions for now, so we can discard unecessary DllImports

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Function_List sass_make_function_list(size_t length);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Function_Entry sass_make_function(StringUtf8 signature, Sass_Function_Fn cb, void* cookie);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Function_Entry sass_function_get_list_entry(Sass_Function_List list, size_t pos);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_function_set_list_entry(Sass_Function_List list, size_t pos,
            Sass_Function_Entry cb);

        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_function_get_signature(Sass_Function_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Function_Fn sass_function_get_function(Sass_Function_Entry cb);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void* sass_function_get_cookie(Sass_Function_Entry cb);

        // libsass\include\sass\values.h (56 hits)
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Tag sass_value_get_tag(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_null(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_number(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_string(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_boolean(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_color(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_list(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_map(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_error(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_value_is_warning(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_number_get_value(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_number_set_value(Sass_Value* v, double value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_number_get_unit(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_number_set_unit(Sass_Value* v, StringUtf8 unit);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_string_get_value(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_string_set_value(Sass_Value* v, StringUtf8 value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_string_is_quoted(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_string_set_quoted(Sass_Value* v, bool quoted);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern bool sass_boolean_get_value(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_boolean_set_value(Sass_Value* v, bool value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_color_get_r(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_color_set_r(Sass_Value* v, double r);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_color_get_g(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_color_set_g(Sass_Value* v, double g);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_color_get_b(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_color_set_b(Sass_Value* v, double b);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern double sass_color_get_a(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_color_set_a(Sass_Value* v, double a);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_list_get_length(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Separator sass_list_get_separator(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_list_set_separator(Sass_Value* v, Sass_Separator value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_list_get_value(Sass_Value* v, size_t i);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_list_set_value(Sass_Value* v, size_t i, Sass_Value* value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern size_t sass_map_get_length(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_map_get_key(Sass_Value* v, size_t i);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_map_set_key(Sass_Value* v, size_t i, Sass_Value* value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_map_get_value(Sass_Value* v, size_t i);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_map_set_value(Sass_Value* v, size_t i, Sass_Value* value);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_error_get_message(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_error_set_message(Sass_Value* v, StringUtf8 msg);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern StringUtf8 sass_warning_get_message(Sass_Value* v);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_warning_set_message(Sass_Value* v, StringUtf8 msg);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_null();
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_boolean(bool val);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_string(StringUtf8 val);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_qstring(StringUtf8 val);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_number(double val, StringUtf8 unit);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_color(double r, double g, double b, double a);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_list(size_t len, Sass_Separator sep);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_map(size_t len);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_error(StringUtf8 msg);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_make_warning(StringUtf8 msg);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern void sass_delete_value(Sass_Value* val);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_clone_value(Sass_Value* val);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_value_stringify(Sass_Value* a, bool compressed, int precision);
        [DllImport(LibSassDll, CallingConvention = CallingConvention.Cdecl)]public static extern Sass_Value* sass_value_op(Sass_OP op, Sass_Value* a, Sass_Value* b);
        */

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LibSass.Sass_Import_List sass_importer_delegate(
            LibSass.StringUtf8 cur_path, LibSass.Sass_Importer_Entry cb, LibSass.Sass_Compiler compiler);

        public struct Sass_Context
        {
            public Sass_Context(IntPtr pointer)
            {
                this.Pointer = pointer;
            }

            public readonly IntPtr Pointer;
        }

        public struct Sass_Options
        {
            public readonly IntPtr Pointer;
        }

        public struct Sass_File_Context
        {
            public Sass_File_Context(IntPtr pointer)
            {
                this.Pointer = pointer;
            }

            public readonly IntPtr Pointer;

            public static implicit operator Sass_Context(Sass_File_Context context)
            {
                return new Sass_Context(context.Pointer);
            }
            public static explicit operator Sass_File_Context(Sass_Context context)
            {
                return new Sass_File_Context(context.Pointer);
            }
        }

        public struct Sass_Data_Context
        {
            public Sass_Data_Context(IntPtr handle)
            {
                this.Pointer = handle;
            }

            public readonly IntPtr Pointer;

            public static implicit operator Sass_Context(Sass_Data_Context context)
            {
                return new Sass_Context(context.Pointer);
            }
            public static explicit operator Sass_Data_Context(Sass_Context context)
            {
                return new Sass_Data_Context(context.Pointer);
            }
        }
        public struct Sass_Compiler
        {
            public Sass_Compiler(IntPtr pointer)
            {
                Pointer = pointer;
            }

            public readonly IntPtr Pointer;
        }

        public struct Sass_Importer_List
        {

            public readonly IntPtr Pointer;
        }

        public struct Sass_Function_List
        {
            public readonly IntPtr Pointer;
        }

        public struct Sass_Import_Entry
        {
            public Sass_Import_Entry(IntPtr pointer)
            {
                Pointer = pointer;
            }

            public readonly IntPtr Pointer;
        }

        public struct Sass_Import_List
        {
            public readonly IntPtr Pointer;
        }

        public struct Sass_Importer_Entry
        {
            public readonly IntPtr Pointer;
        }

        public struct Sass_Importer_Fn
        {
            public Sass_Importer_Fn(IntPtr pointer)
            {
                Pointer = pointer;
            }

            public readonly IntPtr Pointer;
        }

        public struct Sass_Function_Entry
        {
            public readonly IntPtr Pointer;
        }

        public struct Sass_Function_Fn
        {
            public readonly IntPtr Pointer;
        }

        public enum Sass_Compiler_State
        {
        }

        public enum Sass_Tag
        {
        }

        public enum Sass_Separator
        {
        }

        public struct Sass_Value
        {
        }

        public enum Sass_OP
        {
        }

        public struct size_t
        {
            public size_t(int value)
            {
                this.value = new IntPtr(value);
            }

            public size_t(long value)
            {
                this.value = new IntPtr(value);
            }

            private IntPtr value;

            public static implicit operator long(size_t size)
            {
                return size.value.ToInt64();
            }

            public static implicit operator int(size_t size)
            {
                return size.value.ToInt32();
            }

            public static implicit operator size_t(long size)
            {
                return new size_t(size);
            }

            public static implicit operator size_t(int size)
            {
                return new size_t(size);
            }
        }

        public struct StringUtf8
        {
            public StringUtf8(IntPtr pointer)
            {
                this.pointer = pointer;
            }

            private readonly IntPtr pointer;

            public bool IsEmpty => pointer == IntPtr.Zero;

            public static implicit operator string(StringUtf8 stringUtf8)
            {
                if (stringUtf8.pointer != IntPtr.Zero)
                {
                    return Utf8ToString(stringUtf8.pointer);
                }
                return null;
            }

            public static unsafe implicit operator StringUtf8(string text)
            {
                if (text == null)
                {
                    return new StringUtf8(IntPtr.Zero);
                }

#if SUPPORT_UTF8_GETBYTES_UNSAFE
                var length = Encoding.UTF8.GetByteCount(text);
                var pointer = sass_string_alloc(new IntPtr(length));
                fixed (char* pText = text)
                    Encoding.UTF8.GetBytes(pText, text.Length, (byte*) pointer, length);
#else

                var buffer = Encoding.UTF8.GetBytes(text);
                var length = buffer.Length;
                var pointer = sass_string_alloc(new IntPtr(length));
                Marshal.Copy(buffer, 0, pointer, length);
#endif
                ((byte*)pointer)[length] = 0;
                return new StringUtf8(pointer);
            }

            private static unsafe string Utf8ToString(IntPtr utfString)
            {
                var pText = (byte*)utfString;
                // find the end of the string
                while (*pText != 0)
                {
                    pText++;
                }
                int length = (int)(pText - (byte*)utfString);
#if SUPPORT_UTF8_GETSTRING_UNSAFE
                var text = Encoding.UTF8.GetString((byte*)utfString, length);
#else
                // should not be null terminated
                byte[] strbuf = new byte[length]; // TODO: keep a buffer bool
                // skip the trailing null
                Marshal.Copy(utfString, strbuf, 0, length);
                var text = Encoding.UTF8.GetString(strbuf,0, length);
#endif
                return text;
            }
        }
    }
}