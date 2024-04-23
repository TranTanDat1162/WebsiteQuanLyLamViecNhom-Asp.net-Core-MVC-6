const path = require('path');

module.exports = {
    mode: 'development',

    devtool: 'inline-source-map',

    entry: ['babel-polyfill', './MyUI/Components/MyUIMain.js'],

    output: {
        filename: 'myui-bundle.js',
        path: path.join(__dirname, 'wwwroot/dist')
    },
    module: {
        rules: [{
            loader: 'babel-loader',
            test: /\.js$/,
            exclude: /node_modules/
        }]
    }
};