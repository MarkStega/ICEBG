/**
 * @license
 * Copyright 2024 Materia Technologies, Inc.
 */

import summary from 'rollup-plugin-summary';
import resolve from '@rollup/plugin-node-resolve';
import replace from '@rollup/plugin-replace';

export default {
    input: 'BlazorScripts/Materia.Blazor.js',
    output: {
        file: 'wwwroot/Materia.Blazor.js',
        format: 'esm',
    },
    onwarn(warning) {
        if (warning.code !== 'THIS_IS_UNDEFINED') {
            console.error(`(!) ${warning.message}`);
        }
    },
    plugins: [
        replace({ 'Reflect.decorate': 'undefined' }),
        resolve(),
        summary,
    ],
};
