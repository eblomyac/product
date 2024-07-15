const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:14189';

const PROXY_CONFIG = [
  {
    context: [
      "/users",
      "/works",
      "/posts",
      "/swagger",
      "/issues",
      "/analytic",
      "/transfer",
      "/techcard",
      "/priority",
      "/download",
      "/DailySource",
      "/AdditionalCost",
      "/Info"
   ],
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]

module.exports = PROXY_CONFIG;
