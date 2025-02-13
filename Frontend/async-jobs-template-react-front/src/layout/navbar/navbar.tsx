/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { useIsAuthenticated } from '@azure/msal-react';
import { ScrollArea } from '@mantine/core';
import { LinksGroup } from '../navbar-links-group/navbar-links-group';
import { UserInfo } from '../user-info/user-info';
import classes from './navbar.module.css';

const menuDataAnonymous = [
  {
    label: 'Sign In',
    currentLink: '/',
  },
];

const menuDataLoggedIn = [
  {
    label: 'Home Page',
    currentLink: '/',
  },
  {
    label: 'Jobs',
    initiallyOpened: true,
    links: [
      { label: 'Job 1', link: '/jobs/job1' },
      { label: 'Job 2', link: '/jobs/job2' },
      { label: 'Job 3', link: '/jobs/job3' },
    ],
  },
];

export function Navbar() {
  const isAuthentication = useIsAuthenticated();

  const menuData = isAuthentication ? menuDataLoggedIn : menuDataAnonymous;
  const links = menuData.map((item) => <LinksGroup {...item} key={item.label} />);

  return (
    <nav className={classes.navbar}>
      <ScrollArea className={classes.links}>
        <div className={classes.linksInner}>{links}</div>
      </ScrollArea>

      <div className={classes.footer}>
        <UserInfo />
      </div>
    </nav>
  );
}
