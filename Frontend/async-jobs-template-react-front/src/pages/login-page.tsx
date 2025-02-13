import { useMsal } from '@azure/msal-react';
import { Button, Text } from '@mantine/core';

export function LoginPage() {
  const { instance } = useMsal();

  const handleSignIn = async () => {
    await instance.loginRedirect();
  };

  return (
    <>
      <Text mb="1rem">Please sign-in.</Text>
      <Button variant="filled" onClick={handleSignIn}>
        Sign In
      </Button>
    </>
  );
}
