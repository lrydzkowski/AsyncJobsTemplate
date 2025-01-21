// import { Outlet } from 'react-router';
import { AppShell, Burger, Flex, Title } from '@mantine/core';
// import { Navbar } from './layout/navbar/navbar';
// import classes from './app.module.css';
import { useDisclosure } from '@mantine/hooks';

export function App() {
  const [opened, { toggle }] = useDisclosure();

  return (
    // <div className={classes.layout}>
    //   <div className={classes.leftCol}>
    //     <Navbar />
    //   </div>
    //   <div className={classes.rightCol}>
    //     <Outlet />
    //   </div>
    // </div>
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding="md">
      <AppShell.Header pl="md" pr="md">
        <Flex h="100%" align="center" direction="row" wrap="wrap">
          <Burger mr="md" opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Title order={1} size="h3">
            async-jobs-template
          </Title>
        </Flex>
      </AppShell.Header>

      <AppShell.Navbar p="md">Navbar</AppShell.Navbar>

      <AppShell.Main>Main</AppShell.Main>
    </AppShell>
  );
}
