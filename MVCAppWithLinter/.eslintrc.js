module.exports = {
  // Specifies the ESLint parser
  parserOptions: {
    ecmaVersion: 2020, // Allows for the parsing of modern ECMAScript features (e.g., async/await, optional chaining)
    sourceType: 'module', // Allows for the use of ES module imports/exports
  },
  // Specifies the environments your code runs in
  env: {
    browser: true, // Enables browser global variables (e.g., window, document, console)
    es2020: true,  // Enables ES2020 global variables and parsing for the specified ECMAScript version
    // If you're also running in Node.js (e.g., for build scripts or server-side rendering), uncomment this:
    // node: true, // Enables Node.js global variables and Node.js scoping
  },
  // Extends recommended configurations
  extends: [
    'eslint:recommended',       // ESLint's core recommended rules
    'plugin:jsdoc/recommended', // Recommended JSDoc rules
  ],
  // Plugins used in this configuration
  plugins: [
    'jsdoc',
  ],
  // Custom rules for ESLint and JSDoc
  rules: {
    // --- ESLint Core Rules (Recommended adjustments for cleaner code) ---
    'indent': ['error', 2, { 'SwitchCase': 1 }], // Enforce 2-space indentation, 1 level for switch cases
    'linebreak-style': ['error', 'unix'],     // Enforce Unix-style line endings
    'quotes': ['error', 'single', { 'avoidEscape': true, 'allowTemplateLiterals': true }], // Enforce single quotes, allow template literals
    'semi': ['error', 'always'],              // Enforce semicolons at the end of statements
    'no-unused-vars': ['warn', { 'argsIgnorePattern': '^_', 'varsIgnorePattern': '^_' }], // Warn about unused variables, ignore those starting with _
    'no-console': ['warn', { 'allow': ['warn', 'error'] }], // Warn about console.log, allow console.warn and console.error
    'arrow-parens': ['error', 'as-needed', { 'requireForBlockBody': true }], // Require parens in arrow function arguments only when necessary, unless block body
    'eqeqeq': ['error', 'always', { 'null': 'ignore' }], // Enforce strict equality (===, !==) except for null comparisons
    'no-trailing-spaces': 'error',           // Disallow trailing whitespace at the end of lines
    'comma-dangle': ['error', 'always-multiline'], // Require trailing commas for multi-line arrays/objects
    'no-multi-spaces': 'error',               // Disallow multiple spaces
    'object-curly-spacing': ['error', 'always'], // Enforce consistent spacing inside braces of objects
    'array-bracket-spacing': ['error', 'never'], // Disallow spaces inside array brackets

    // --- JSDoc Rules (Strict enforcement for all functions) ---
    'jsdoc/check-param-names': 'error',          // Ensures param names in JSDoc match function params
    'jsdoc/check-tag-names': 'error',            // Ensures valid JSDoc tag names
    'jsdoc/empty-tags': 'error',                 // No unnecessary text in empty JSDoc tags
    'jsdoc/require-param': 'error',              // **Requires all parameters to be documented**
    'jsdoc/require-param-description': 'error',  // **Requires descriptions for all parameters**
    'jsdoc/require-returns': ['error', { 'forceReturnsWithAsync': true }], // **Requires return documentation for functions that return a value**
    'jsdoc/require-returns-description': 'error', // **Requires descriptions for return values**
    'jsdoc/valid-types': 'error',                // Ensures JSDoc types are syntactically valid
    'jsdoc/check-types': 'error',                // Ensures JSDoc types are valid and optionally exist

    // Enforce JSDoc presence for ALL functions
    'jsdoc/require-jsdoc': ['error', {
      'require': {
        'FunctionDeclaration': true,     // Enforce JSDoc for regular functions
        'MethodDefinition': true,        // Enforce JSDoc for class methods
        'ClassDeclaration': true,        // Enforce JSDoc for classes
        'ArrowFunctionExpression': true, // **Enforce JSDoc for arrow functions**
        'FunctionExpression': true,      // Enforce JSDoc for anonymous function expressions
      },
    }],

    // Optional JSDoc rules for even more strictness:
    'jsdoc/require-description': ['error', { 'checkDecorators': false }], // Requires a description for the JSDoc block itself
    'jsdoc/require-file-overview': 'off', // 'error' if you want a JSDoc block at the top of every file
    'jsdoc/check-alignment': 'error',     // Enforce consistent alignment of tags
    'jsdoc/no-types': 'off', // Set to 'error' if you are using TypeScript and don't want JSDoc @type tags
    'jsdoc/require-throws': 'off', // 'error' if you want to document all thrown errors
    'jsdoc/require-example': 'off', // 'error' if you want to require code examples
    'jsdoc/check-examples': 'off', // 'error' to lint code examples in JSDoc (requires `eslint-plugin-jsdoc` setup)
  },
  // Settings for plugins
  settings: {
    jsdoc: {
      tagNamePreference: {
        returns: 'return', // Prefer @return over @returns
      },
      // If you are using specific JSDoc type definitions (e.g., from a library)
      // you can list them here:
      // see: https://github.com/gajus/eslint-plugin-jsdoc#user-content-global-and-typedef-contexts
      // definedTypes: ['MyCustomType'],
    },
  },
};