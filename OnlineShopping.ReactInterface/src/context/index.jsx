import React, { createContext, useState } from 'react';
import PropTypes from 'prop-types';

const Context = createContext();

const Provider = ({
  children,
}) => {

  const [view, setView] = useState('menu');

  return (
    <Context.Provider
      value={{
        view,
        setView,
      }}
    >
      {children}
    </Context.Provider>
  );

};

Provider.propTypes = {
  children: PropTypes.node,
};

export {
  Context,
  Provider,
};