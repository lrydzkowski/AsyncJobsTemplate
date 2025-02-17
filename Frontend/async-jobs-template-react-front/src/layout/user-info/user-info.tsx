import { useMsal } from '@azure/msal-react';
import { IconLogout } from '@tabler/icons-react';
import { Box, Group, Text, Tooltip, UnstyledButton } from '@mantine/core';
import classes from './user-info.module.css';

export function UserInfo() {
  const { instance, accounts } = useMsal();

  const isLoggedIn = accounts?.length > 0;

  const logout = async () => {
    await instance.logoutRedirect();
  };

  const username = accounts[0]?.username ?? 'anonymous';

  return !isLoggedIn ? (
    <Box className={classes.user}>
      <Text size="sm" fw={500} truncate="end">
        Version: 1.0.2
      </Text>
    </Box>
  ) : (
    <>
      <Box className={classes.user}>
        <Tooltip label={username}>
          <Text size="sm" fw={500} truncate="end">
            {username}
          </Text>
        </Tooltip>
      </Box>
      <UnstyledButton onClick={logout} className={classes.control}>
        <Group gap={0}>
          <IconLogout className={classes.linkIcon} stroke={1.5} />
          <Box style={{ display: 'flex', alignItems: 'center' }}>
            <Box ml="md">Logout</Box>
          </Box>
        </Group>
      </UnstyledButton>
    </>
  );
}
