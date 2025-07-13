const {
    defineConfig,
} = require("eslint/config");

const globals = require("globals");
const jsdoc = require("eslint-plugin-jsdoc");
const js = require("@eslint/js");
const htmlParser = require("@html-eslint/parser");
const htmlPlugin = require("@html-eslint/eslint-plugin");

const {
    FlatCompat,
} = require("@eslint/eslintrc");

const compat = new FlatCompat({
    baseDirectory: __dirname,
    recommendedConfig: js.configs.recommended,
    allConfig: js.configs.all
});

function commonConfig() {
    return {
        files: ["wwwroot/js/**/*.js"],
        languageOptions: {
            ecmaVersion: 2020,
            sourceType: "module",
            parserOptions: {},

            globals: {
                ...globals.browser,
            },
        },

        extends: compat.extends("eslint:recommended", "plugin:jsdoc/recommended"),

        plugins: {
            jsdoc,
        },

        rules: {
            "indent": ["error", 4, {
                "SwitchCase": 1,
            }],

            "semi": ["error", "always"],

            "no-unused-vars": ["warn", {
                "argsIgnorePattern": "^_",
                "varsIgnorePattern": "^_",
            }],

            "no-console": ["warn", {
                "allow": ["warn", "error"],
            }],

            "arrow-parens": ["error", "as-needed", {
                "requireForBlockBody": true,
            }],

            "eqeqeq": ["error", "always", {
                "null": "ignore",
            }],

            "no-trailing-spaces": "error",
            "comma-dangle": ["error", "always-multiline"],
            "no-multi-spaces": "error",
            "object-curly-spacing": ["error", "always"],
            "array-bracket-spacing": ["error", "never"],
            "jsdoc/check-param-names": "error",
            "jsdoc/check-tag-names": "error",
            "jsdoc/empty-tags": "error",
            "jsdoc/require-param": "error",
            "jsdoc/require-param-description": "error",

            "jsdoc/require-returns": ["error", {
                "forceReturnsWithAsync": true,
            }],

            "jsdoc/require-returns-description": "error",
            "jsdoc/valid-types": "error",
            "jsdoc/check-types": "error",

            "jsdoc/require-jsdoc": ["error", {
                "require": {
                    "FunctionDeclaration": true,
                    "MethodDefinition": true,
                    "ClassDeclaration": true,
                    "ArrowFunctionExpression": true,
                    "FunctionExpression": true,
                },
            }],

            "jsdoc/require-file-overview": "off",
            "jsdoc/check-alignment": "error",
            "jsdoc/no-types": "off",
            "jsdoc/require-throws": "off",
            "jsdoc/require-example": "off",
            "jsdoc/check-examples": "off",
        },

        settings: {
            jsdoc: {
                tagNamePreference: {
                    returns: "returns",
                },
            },
        },
    };
}

module.exports = defineConfig([{
    files: ["wwwroot/js/**/*.js"],
    ...commonConfig(),
}]);