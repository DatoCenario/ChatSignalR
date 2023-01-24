import React from "react";
import { Messenger } from "../../src/components/main";
import Head from "next/head";

const IndexPage = function IndexPage() {
  return (
    <div>
      <Head>
        <title>My page title</title>
        <body style={{
            overflow: 'hidden'
        }} />
      </Head>
      <Messenger />
    </div>
  );
}

IndexPage.isPrivate = true;

export default IndexPage;
