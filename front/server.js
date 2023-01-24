const express = require('express')
const next = require('next')
const { createProxyMiddleware } = require("http-proxy-middleware")
require('dotenv').config();


const port = process.env.FRONTEND_PORT;
const dev = process.env.NODE_ENV !== 'production'
const app = next({ dev })
const handle = app.getRequestHandler()

const targetUrl = (new URL(`http://${process.env.BACKEND_HOST_URL}:${process.env.BACKEND_PORT}`)).origin

const apiPaths = {
    '/api': {
        target: targetUrl, 
        pathRewrite: {
            '^/api': '/api'
        },
        changeOrigin: true,
        router: function () {
          return targetUrl
      },
    }
}

const isDevelopment = process.env.NODE_ENV !== 'production'

app.prepare().then(() => {
  const server = express();
 
  if (isDevelopment) {
    server.use('/api', createProxyMiddleware(apiPaths['/api']));

    server.use(
      createProxyMiddleware('/hub', {
        target: targetUrl,
        ws: true,
      })
    );
  }

  server.all('*', (req, res) => {
    return handle(req, res)
  })

  server.listen(port, process.env.FRONTEND_HOST_URL, (err) => {
    if (err) throw err
    console.log(`> Ready on ${process.env.FRONTEND_HOST_URL}:${process.env.FRONTEND_PORT}`)
  })
}).catch(err => {
    console.log('Error:::::', err)
})