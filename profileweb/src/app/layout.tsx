import React, { ReactNode } from 'react';

import './styles/layout.scss';  // Importe seus estilos SCSS
import './styles/globals.scss';  // Importe seus estilos SCSS

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <html>
      <head>
        <title>Hélio Martins - Profile</title>
      </head>
      <body>
        {children}
      </body>
    </html>
  );
};

export default Layout;
