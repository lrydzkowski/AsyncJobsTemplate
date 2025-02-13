import { InteractionStatus } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { Outlet } from 'react-router';
import { AppShell, Box, Burger, Flex, Text, Title } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Navbar } from './layout/navbar/navbar';

export function App() {
  const { inProgress } = useMsal();
  const [opened, { toggle }] = useDisclosure();

  const isLoading = inProgress === InteractionStatus.Logout;

  return (
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
          <Title order={1} size="h3" c="blue">
            async-jobs-template
          </Title>
        </Flex>
      </AppShell.Header>

      <AppShell.Navbar>{!isLoading && <Navbar />}</AppShell.Navbar>

      <AppShell.Main>
        <Box>{isLoading ? <Text>Loading...</Text> : <Outlet />}</Box>
      </AppShell.Main>
    </AppShell>
  );
}
