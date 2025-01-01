import { useMsal } from '@azure/msal-react';

export function LoginPage() {
  const { instance } = useMsal();

  const handleSignIn = async () => {
    await instance.loginRedirect();
  };

  return (
    <>
      <p>Please sign-in.</p>
      <button onClick={handleSignIn}>Sign In</button>
    </>
  );
}
