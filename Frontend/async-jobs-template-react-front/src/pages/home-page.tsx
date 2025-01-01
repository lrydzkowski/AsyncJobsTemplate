import { useMsal } from '@azure/msal-react';

export function HomePage() {
  const { instance, accounts } = useMsal();

  console.log(accounts);

  const handleSignOut = async () => {
    await instance.logoutRedirect();
  };

  return (
    <>
      <p>Home Page</p>
      <button onClick={handleSignOut}>Sign Out</button>
    </>
  );
}
