import { Text, Title } from '@mantine/core';

export default function NotFoundPage() {
  return (
    <>
      <Title order={2} c="red" size="h3">
        Error 404
      </Title>
      <Text>Page not found</Text>
    </>
  );
}
