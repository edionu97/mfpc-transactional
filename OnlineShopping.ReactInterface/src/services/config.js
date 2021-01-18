const config = {
  IP: 'localhost',
  PORT: 59356,
};

// http://localhost:59356/api/clients/all

const URI = (route) => `http://${config.IP}:${config.PORT}/api/${route}`;

export {
  URI,
};